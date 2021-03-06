library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;
library std;
use std.textio.all;

entity controller is
	port (	
		clk		: in	std_logic;
		reset	: in	std_logic;

		sensor		: in	std_logic_vector(2 downto 0);
		minedetected	: in	std_logic;		

		count_in		: in	unsigned (19 downto 0);
		count_reset		: out	std_logic;

		motor_l_reset		: out	std_logic;
		motor_l_speed		: out	signed (7 downto 0);

		motor_r_reset		: out	std_logic;
		motor_r_speed		: out	signed (7 downto 0);
		
		bin_seg					: out std_logic_vector (15 downto 0);
		dpoint_seg					: out std_logic_vector(3 downto 0);
		led					: out std_logic_vector (7 downto 0);	
		sw					: in std_logic_vector (7 downto 0);		
		
		uart_send				: out std_logic_vector(7 downto 0);
		uart_receive			: in std_logic_vector(7 downto 0);
		uart_rw_out				: out std_logic_vector(1 downto 0); -- 0 = read, 1 = write
		uart_br 				: in std_logic
	);
end entity controller;

architecture b of controller is	
	subtype byte is std_logic_vector (7 downto 0);
	--protocol Rev. E
	constant p_forward	: byte := x"46"; -- 'F'
	constant p_stop		: byte := x"53"; -- 'S'
	constant p_left		: byte := x"4C"; -- 'L'
	constant p_right	: byte := x"52"; -- 'R'
	constant p_turn		: byte := x"54"; -- 'T'
	constant p_back		: byte := x"42"; -- 'B'
	constant p_half		: byte := x"48"; -- 'H'
	constant p_cont		: byte := x"01"; -- SOH
	constant p_ack		: byte := x"06"; -- ACK
	constant p_nak		: byte := x"15"; -- NAK
	constant p_enq		: byte := x"05"; -- ENQ
	constant p_mine		: byte := x"07"; -- BEL
	constant p_done		: byte := x"04"; -- EOT
	constant p_unknown	: byte := x"00"; -- NULL
	
	type sys_state is (followline, processnextturn, leftturn, rightturn, fullturn, turnback, callforinput, sendmine, sendhalf, waitforinput, sendok, sendfail, arewedone, done);
	type sender_state is (swaiting, ssending, ssetwrite, sunsetwrite);
	type receiver_state is (rwaiting, rreceiving, rsetread, runsetread);
	signal state : sys_state := followline;
	signal turndelay : integer; 
	signal fullturndelay : integer; 
	signal failsafedelay : integer; 
	signal delaycounter : integer;
	signal failsafecounter : integer;
	signal sstate : sender_state := swaiting;
	signal rstate : receiver_state := rwaiting;
	signal uart_rw : std_logic_vector(1 downto 0);
	signal packetcounter: unsigned(7 downto 0);
	signal nextturn : unsigned(2 downto 0) := to_unsigned(1,3); -- 0 = left, 1 = forward, 2 = right, 3 = stop, 4 = turn
	signal sending : std_logic := '0'; -- internal
	signal rresponse, sresponse : std_logic_vector (1 downto 0) := "00";
	signal passedminesite : std_logic;
	signal fastmode : std_logic;
	signal notafraid : std_logic; --when the robot is not afraid it doesn't care about driving over mines.
	--signal turnprocessed : std_logic;
	signal isdone, continue : std_logic;
begin
	
	led(7 downto 0)<=std_logic_vector(to_unsigned(delaycounter, 32)(26 downto 19));
	--led(4 downto 3)<=rresponse;
	--led(5)<=minedetected;
	--led(7 downto 6)<=uart_rw;	
	dpoint_seg(3 downto 0)<="0000";
	uart_rw_out<=uart_rw;	
	fastmode<=sw(0);
	notafraid<=sw(1);
	
	process (clk) is
		variable next_state : sys_state;
		variable debugid : unsigned ( 7 downto 0);
		variable next_sending :std_logic;
		variable next_delaycounter : integer;
		variable next_failsafecounter : integer;
		variable next_passedminesite : std_logic;
	begin	
		if rising_edge(clk) then
			--default values
			motor_l_speed <= to_signed(0,8);
			motor_r_speed <= to_signed(0,8);
			--bin_seg(11 downto 4) <= "11111111";
			next_state:=state;
			next_sending:='0';
			next_passedminesite:=passedminesite;
			if fastmode = '0' then
				turndelay<=20000000;
				fullturndelay<=80000000;
				failsafedelay<=100000000;
			else
				turndelay<=13000000;
				fullturndelay<=32000000;
				failsafedelay<=66000000;
			end if;
			if reset = '1' then
				uart_send <= p_unknown;
				next_state:=followline;				
				debugid:=to_unsigned(0,8);
				next_delaycounter:=1000;
				next_passedminesite:='1';
				next_failsafecounter := 0;				
				--turnprocessed <= '1';				
			elsif state = arewedone then
				debugid:=to_unsigned(16#B#,8);
				if isdone = '1' then
					next_state:=done;
				elsif continue = '1' then
					next_passedminesite:='1';
					next_delaycounter:=1337;
					next_state:=callforinput;
				else
					next_state:=followline;
				end if;
			elsif state = done then
				debugid:=to_unsigned(16#A#,8);
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
			elsif state = followline then
				debugid:=to_unsigned(16#1#,8);			
				--follow line
				if delaycounter > 0 then
					debugid:=to_unsigned(16#C#,8);
					next_delaycounter:=delaycounter-1;
				end if;
				if (passedminesite = '0') then	
					next_failsafecounter:=failsafecounter+1;
				end if;
				if (failsafecounter > failsafedelay) and (passedminesite = '0') then -- TODO estimate correct number.			
					next_passedminesite:='1';
					next_state:=sendhalf;				
				end if;
				if minedetected = '1' then
					next_state:=sendmine;
					next_passedminesite:= '0';
					next_failsafecounter := 0;
				else				
					case sensor is
						when "000" => 
							motor_l_speed <= to_signed(100,8);
							motor_r_speed <= to_signed(100,8);
							if delaycounter = 0 then
								if passedminesite = '1' then
									if fastmode = '1' then
										next_delaycounter:=13000000;
									else
										next_delaycounter:=20000000;
									end if;
									next_state:=callforinput;
									next_passedminesite:='0';
									next_failsafecounter := 0;
								else
									if fastmode = '1' then
										next_delaycounter:=13000000;
									else
										next_delaycounter:=20000000;
									end if;
									next_passedminesite:='1';
									next_state:=sendhalf;
									next_failsafecounter := 0;
								end if;
							end if;						
						when "001" => motor_l_speed <= to_signed(20,8); motor_r_speed <= to_signed(100,8);
						when "010" => motor_l_speed <= to_signed(50,8); motor_r_speed <= to_signed(50,8);
						when "011" => motor_l_speed <= to_signed(-50,8); motor_r_speed <= to_signed(100,8);
						when "100" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(20,8);
						when "101" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8);
						when "110" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(-50,8);
						when "111" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8); next_state:=arewedone;
						when others => motor_l_speed <= to_signed(0,8); motor_r_speed <= to_signed(0,8);
				   end case;
				end if;
			elsif state = processnextturn then
				debugid:=to_unsigned(16#2#,8);
				motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8);			
					if nextturn = 0 then
						if fastmode = '1' then
							next_delaycounter:=26000000;
						else
							next_delaycounter:=40000000;
						end if;						
						next_state:=leftturn; --left
					elsif nextturn = 1 then
						next_state:=followline; --forward (line)
					elsif nextturn = 2 then	
						if fastmode = '1' then
							next_delaycounter:=26000000;
						else
							next_delaycounter:=40000000;
						end if;							
						next_state:=rightturn; --right
					elsif nextturn = 3 then
						next_state:=callforinput; --stop (wait for input)
					elsif nextturn = 4 then
						if (delaycounter > 1337) or (delaycounter < 999) then
							if fastmode = '1' then
								next_delaycounter:=40000000;
							else
								next_delaycounter:=100000000;
							end if;	
						end if;						
						next_state:=fullturn; --turn
					elsif nextturn = 5 then
						next_state:=turnback; --back
					end if;
				
			elsif state = leftturn then
				debugid:=to_unsigned(16#3#,8);
				--left
				if delaycounter > 0 then
					debugid:=to_unsigned(16#F#,8);
					next_delaycounter:=delaycounter-1;
				end if;
				if delaycounter > turndelay then					
					motor_l_speed <= to_signed(100,8);
					motor_r_speed <= to_signed(100,8);
				elsif (delaycounter < turndelay) and (delaycounter /= 0) then
					motor_l_speed <= to_signed(-100,8);
					motor_r_speed <= to_signed(100,8);
				elsif delaycounter = 0	then
					motor_l_speed <= to_signed(-100,8);
					motor_r_speed <= to_signed(100,8);
					case sensor is			  			  
					  when "101" => next_state:=followline;
					  --when "011" => next_state:=followline;					  
					  when others => --nothing
				    end case;
			   end if;
			elsif state = rightturn then
				debugid:=to_unsigned(16#4#,8);
				--left
				if delaycounter > 0 then
					debugid:=to_unsigned(16#10#,8);
					next_delaycounter:=delaycounter-1;
				end if;
				if delaycounter > turndelay then					
					motor_l_speed <= to_signed(100,8);
					motor_r_speed <= to_signed(100,8);
				elsif (delaycounter < turndelay) and (delaycounter /= 0) then
					motor_l_speed <= to_signed(100,8);
					motor_r_speed <= to_signed(-100,8);
				elsif delaycounter = 0	then
					motor_l_speed <= to_signed(100,8);
					motor_r_speed <= to_signed(-100,8);
					case sensor is			  			  
					  when "101" => next_state:=followline;
					 -- when "110" => next_state:=followline;
					  when others => --nothing
				    end case;
			    end if;
			elsif state = fullturn then
				debugid:=to_unsigned(16#9#,8);
				--full turn
				if delaycounter > 0 then
					debugid:=to_unsigned(16#11#,8);
					next_delaycounter:=delaycounter-1;
				end if;
				if delaycounter >= fullturndelay then					
					motor_l_speed <= to_signed(100,8);
					motor_r_speed <= to_signed(100,8);
				elsif (delaycounter < fullturndelay) and (delaycounter /= 0) then
					motor_l_speed <= to_signed(-100,8);
					motor_r_speed <= to_signed(100,8);
				elsif delaycounter = 0	then				
					motor_l_speed <= to_signed(-100,8);
					motor_r_speed <= to_signed(100,8);	
					case sensor is	
						when "101" => next_state:=followline;
						when others => --nothing
					end case;
				end if;			   
			elsif state = turnback then
				debugid:=to_unsigned(16#E#,8);
				--backtrack
				case sensor is
						--when "000" => motor_l_speed <= to_signed(-100,8); motor_r_speed <= to_signed(-100,8);											
						--when "001" => motor_l_speed <= to_signed(-100,8); motor_r_speed <= to_signed(-20,8);
						--when "010" => motor_l_speed <= to_signed(50,8); motor_r_speed <= to_signed(50,8);
						--when "011" => motor_l_speed <= to_signed(-100,8); motor_r_speed <= to_signed(50,8);
						--when "100" => motor_l_speed <= to_signed(-20,8); motor_r_speed <= to_signed(-100,8);
						--when "101" => motor_l_speed <= to_signed(-100,8); motor_r_speed <= to_signed(-100,8);
						--when "110" => motor_l_speed <= to_signed(50,8); motor_r_speed <= to_signed(100,8);
						--when "111" => motor_l_speed <= to_signed(-100,8); motor_r_speed <= to_signed(-100,8);
						when others => motor_l_speed <= to_signed(-100,8); motor_r_speed <= to_signed(-100,8);
				   end case;
				if delaycounter > 0 then
					debugid:=to_unsigned(16#12#,8);
					next_delaycounter:=delaycounter-1;
				end if;
				if delaycounter = 0	then			
					case sensor is															
					  when "000" => 
						--next_delaycounter:=0000000;
					  	next_state := callforinput;
						next_passedminesite := '0';
					  when others => --nothing
				   end case;
			   end if;
			elsif state = callforinput then
				debugid:=to_unsigned(16#5#,8);			
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
				uart_send <= p_enq;
				next_sending:='1';	
				if sresponse = "10" then
					next_state:=waitforinput;	
					next_sending:='0';					
				end if;	
			elsif state = sendmine then
				debugid:=to_unsigned(16#D#,8);
				uart_send <= p_mine;
				next_sending:='1';	
				if sresponse = "10" then
					if notafraid = '1' then						
						if fastmode = '1' then
							next_delaycounter:=13000000;
						else
							next_delaycounter:=20000000;
						end if;
						next_passedminesite:='1';
						next_state:=followline;
					else
						if fastmode = '1' then
							next_delaycounter:=6600000;
						else
							next_delaycounter:=10000000;
						end if;
						next_state:=turnback;
					end if;					
					next_sending:='0';					
				end if;
			elsif state = sendhalf then
				debugid:=to_unsigned(16#E#,8);	
				uart_send <= p_half;
				next_sending:='1';	
				if sresponse = "10" then
					next_state:=followline;	
					next_sending:='0';					
				end if;	
			elsif state = waitforinput then
				debugid:=to_unsigned(16#6#,8);			
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);					
				if rresponse(1) = '1'  then					
					if rresponse(0) = '0' then
						next_state:=sendok;
						--turnprocessed<='0';
						else
						next_state:=sendfail;
					end if;
				end if;	
			elsif state = sendok then
				debugid:=to_unsigned(16#7#,8);			
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
				uart_send <= p_ack;
				next_sending:='1';			
				next_state:=processnextturn;								
			elsif state = sendfail then
				debugid:=to_unsigned(16#8#,8);			
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
				uart_send <= p_nak;
				next_sending:='1';			
				next_state:=callforinput;							
			else
				debugid:=to_unsigned(16#FF#,8);
				--stop
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
			end if;		   
			state<=next_state;
			passedminesite<=next_passedminesite;
			sending<=next_sending;
			delaycounter<=next_delaycounter;
			failsafecounter<=next_failsafecounter;
			bin_seg(7 downto 0)<=std_logic_vector(debugid);
		end if;
	end process;
	
	--receiver
	process (clk)
		variable next_state : receiver_state;
		variable next_r : std_logic;
		--variable debugid : unsigned ( 3 downto 0);
		variable response : std_logic_vector (1 downto 0);	
	begin
	if(rising_edge(clk)) then
		next_state:=rstate;
		if reset = '1' then
			--debugid:=to_unsigned(0,4);
			next_r:='0';
			nextturn<=to_unsigned(3,3);
			response:="00";	
			next_state:=rwaiting;
			isdone <= '0';
		elsif rstate = rwaiting then
			--debugid:=to_unsigned(1,4);
			next_r:='0';
			response:="00";			
			if uart_br = '1'  then			
				next_state:=rsetread;		
			end if;	
		elsif rstate = rsetread then
			--debugid:=to_unsigned(2,4);	
			next_r:='1';			
			next_state := rreceiving;
		elsif rstate = runsetread then
			--debugid:=to_unsigned(3,4);	
			next_r:='0';
			next_state := rwaiting;
		elsif rstate = rreceiving then
			--debugid:=to_unsigned(4,4);
			next_r:='1';
			response(1):='1';
			if uart_receive = p_left then
				--left
				response(0):='0';
				nextturn<=to_unsigned(0,3);
			elsif uart_receive = p_right then
				--right
				response(0):='0';
				nextturn<=to_unsigned(2,3);
			elsif uart_receive = p_forward then
				--forward
				response(0):='0';
				nextturn<=to_unsigned(1,3);
			elsif uart_receive = p_stop then
				--stop
				response(0):='0';
				nextturn<=to_unsigned(3,3);
			elsif uart_receive = p_turn then
				--turn
				response(0):='0';
				nextturn<=to_unsigned(4,3);
			elsif uart_receive = p_back then
				--back
				response(0):='0';
				nextturn<=to_unsigned(5,3);
			elsif uart_receive = p_cont then
				--cont
				response(0):='0';
				nextturn<=to_unsigned(3,3);			
			elsif uart_receive = p_done then
				--done
				response(0):='0';
				isdone <= '1';
				nextturn<=to_unsigned(3,3);
			else
				response(0):='1';
				--discard				
			end if;

			if uart_receive = p_cont then
				continue <= '1';
			elsif state = arewedone then
				continue <= '0';
			end if;

			next_state := runsetread;

		end if;		
		uart_rw(0)<=next_r;
		rstate<=next_state;
		rresponse<=response;
		--bin_seg(11 downto 8)<=std_logic_vector(debugid);
		end if;
	end process;
	
	--sender
	process (clk)
		variable next_state : sender_state;
		variable next_w : std_logic;
		variable debugid : unsigned ( 3 downto 0);
		variable response : std_logic_vector (1 downto 0);		
	begin
		if(rising_edge(clk)) then
			next_state:=sstate;
			if reset = '1' then
				--debugid:=to_unsigned(0,4);
				next_w:='0';
				response:="00";	
				next_state:=swaiting;
			elsif sstate = swaiting then
				--debugid:=to_unsigned(1,4);	
				next_w:='0';
				response:="00";	
				if sending = '1'  then			
					next_state:=ssetwrite;
				end if;	
			elsif sstate = ssetwrite then
				--debugid:=to_unsigned(2,4);							
				next_w:='1';
				next_state := ssending;
			elsif sstate = sunsetwrite then
				--debugid:=to_unsigned(3,4);	
				next_w:='0';
				response:="10";			
				next_state := swaiting;
			elsif sstate = ssending then
				--debugid:=to_unsigned(4,4);
				next_w:='0';		
				next_state := sunsetwrite;
			end if;
			uart_rw(1)<=next_w;
			sstate<=next_state;
			sresponse<=response;
			--bin_seg(15 downto 12)<=std_logic_vector(debugid);
		end if;
	end process;
	--counter
	process (clk)
		variable next_pcount : unsigned(7 downto 0);
	begin
		next_pcount:=packetcounter;
		if rising_edge(clk) then
			if reset = '1' then
				next_pcount:=to_unsigned(0,8);
			else 
				if sstate = ssending then
					next_pcount:=next_pcount+1;
				end if;
				if rstate = rreceiving then
					next_pcount:=next_pcount+1;
				end if;
			end if;
			packetcounter<=next_pcount;
		end if;
	end process;
	bin_seg(15 downto 8)<=std_logic_vector(packetcounter);
	motor_l_reset <= reset;
	motor_r_reset <= reset;
	count_reset <= reset;
end architecture b;

