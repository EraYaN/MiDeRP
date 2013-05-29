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
		
		uart_send				: out std_logic_vector(7 downto 0);
		uart_receive			: in std_logic_vector(7 downto 0);
		uart_rw_out				: out std_logic_vector(1 downto 0); -- 0 = read, 1 = write
		uart_br 				: in std_logic
	);
end entity controller;

architecture b of controller is	
	subtype byte is std_logic_vector (7 downto 0);
	--protocol Rev. C
	constant p_forward	: byte := x"46"; -- 'F'
	constant p_stop		: byte := x"53"; -- 'S'
	constant p_left		: byte := x"4C"; -- 'L'
	constant p_right	: byte := x"52"; -- 'R'
	constant p_turn		: byte := x"54"; -- 'T'
	constant p_cont		: byte := x"01"; -- SOH
	constant p_ack		: byte := x"06"; -- ACK
	constant p_nak		: byte := x"15"; -- NAK
	constant p_enq		: byte := x"05"; -- ENQ
	constant p_mine		: byte := x"07"; -- BEL
	constant p_done		: byte := x"04"; -- EOT
	constant p_unknown	: byte := x"00"; -- NULL
	
	type sys_state is (followline, processnextturn, leftturn, rightturn, fullturn, callforinput, sendmine, waitforinput, sendok, sendfail, arewedone, done);
	type sender_state is (swaiting, ssending, ssetwrite, sunsetwrite);
	type receiver_state is (rwaiting, rreceiving, rsetread, runsetread);
	signal state : sys_state := followline;
		
	signal delaycounter : integer;
	signal sstate : sender_state := swaiting;
	signal rstate : receiver_state := rwaiting;
	signal uart_rw : std_logic_vector(1 downto 0);
	signal nextturn : unsigned(2 downto 0) := to_unsigned(1,3); -- 0 = left, 1 = forward, 2 = right, 3 = stop, 4 = turn
	signal sending : std_logic := '0'; -- internal
	signal rresponse, sresponse : std_logic_vector (1 downto 0) := "00";
	signal passedminesite : std_logic;
	--signal turnprocessed : std_logic;
	signal isdone, continue : std_logic;
begin
	
	bin_seg(15 downto 12)<="0000";
	led(2 downto 0)<=std_logic_vector(nextturn);
	led(4 downto 3)<=rresponse;
	led(5)<=minedetected;
	led(7 downto 6)<=uart_rw;	
	dpoint_seg(3 downto 0)<="0000";
	uart_rw_out<=uart_rw;	
	
	process (clk) is
		variable next_state : sys_state;
		variable debugid : unsigned ( 3 downto 0);
		variable next_sending :std_logic;
		variable next_delaycounter : integer;
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
			if reset = '1' then
				uart_send <= p_unknown;
				next_state:=followline;				
				debugid:=to_unsigned(0,4);
				next_delaycounter:=1000000;
				next_passedminesite:='1';
				--turnprocessed <= '1';				
			elsif state = arewedone then
				debugid:=to_unsigned(11,4);
				if isdone = '1' then
					next_state:=done;
				else if continue = '1' then
					next_state:=callforinput;
					continue = '0';
				else
					next_state:=followline;
				end if;
			elsif state = done then
				debugid:=to_unsigned(10,4);
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
			elsif state = followline then
				debugid:=to_unsigned(1,4);			
				--follow line
				if delaycounter > 0 then
					debugid:=to_unsigned(12,4);
					next_delaycounter:=delaycounter-1;
				end if;
				if minedetected = '1' then
					next_state:=sendmine;
					next_passedminesite:= '1';
				else				
					case sensor is
						when "000" => 
							motor_l_speed <= to_signed(100,8);
							motor_r_speed <= to_signed(100,8);
							if delaycounter = 0 then
								if passedminesite = '1' then
									next_delaycounter:=20000000;
									next_state:=callforinput;
									next_passedminesite:='0';
								else
									next_delaycounter:=20000000;
									next_passedminesite:='1';
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
				debugid:=to_unsigned(2,4);
				motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8);			
					if nextturn = 0 then
						next_state:=leftturn; --left
					elsif nextturn = 1 then
						next_state:=followline; --forward (line)
					elsif nextturn = 2 then
						next_state:=rightturn; --right
					elsif nextturn = 3 then
						next_state:=callforinput; --stop (wait for input)
					elsif nextturn = 4 then
						next_state:=fullturn; --turn
					end if;
				
			elsif state = leftturn then
				debugid:=to_unsigned(3,4);
				--left
				motor_l_speed <= to_signed(-25,8);
				motor_r_speed <= to_signed(100,8);
				case sensor is			  			  
				  when "101" => next_state:=followline;
				  when others => --nothing
			   end case;
			elsif state = rightturn then
				debugid:=to_unsigned(4,4);
				--right
				motor_l_speed <= to_signed(100,8);
				motor_r_speed <= to_signed(-25,8);
				case sensor is			  			  
				  when "101" => next_state:=followline;
				  when others => --nothing
			   end case;			   
			elsif state = fullturn then
				debugid:=to_unsigned(9,4);
				--full turn
				motor_l_speed <= to_signed(100,8);
				motor_r_speed <= to_signed(-100,8);
				if delaycounter > 0 then
					debugid:=to_unsigned(12,4);
					next_delaycounter:=delaycounter-1;
				end if;
				if delaycounter = 0	then			
					case sensor is															
					  when "101" => next_state:=followline;
					  when others => --nothing
				   end case;
			   end if;
			elsif state = callforinput then
				debugid:=to_unsigned(5,4);			
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
				uart_send <= p_enq;
				next_sending:='1';	
				if sresponse = "10" then
					next_state:=waitforinput;	
					next_sending:='0';					
				end if;	
			elsif state = sendmine then
				debugid:=to_unsigned(13,4);			
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
				uart_send <= p_mine;
				next_sending:='1';	
				if sresponse = "10" then
					next_delaycounter:=20000000;
					next_state:=fullturn;	
					next_sending:='0';					
				end if;	
			elsif state = waitforinput then
				debugid:=to_unsigned(6,4);			
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
				debugid:=to_unsigned(7,4);			
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
				uart_send <= p_ack;
				next_sending:='1';			
				next_state:=processnextturn;								
			elsif state = sendfail then
				debugid:=to_unsigned(8,4);			
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
				uart_send <= p_nak;
				next_sending:='1';			
				next_state:=callforinput;							
			else
				debugid:=to_unsigned(15,4);
				--stop
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);
			end if;		   
			state<=next_state;
			passedminesite<=next_passedminesite;
			sending<=next_sending;
			delaycounter<=next_delaycounter;
			bin_seg(3 downto 0)<=std_logic_vector(debugid);
		end if;
	end process;
	
	--receiver
	process (clk)
		variable next_state : receiver_state;
		variable next_r : std_logic;
		variable debugid : unsigned ( 3 downto 0);
		variable response : std_logic_vector (1 downto 0);	
	begin
	if(rising_edge(clk)) then
		next_state:=rstate;
		if reset = '1' then
			debugid:=to_unsigned(0,4);
			next_r:='0';
			nextturn<=to_unsigned(3,3);
			response:="00";	
			next_state:=rwaiting;
			isdone <= '0';
		elsif rstate = rwaiting then
			debugid:=to_unsigned(1,4);
			next_r:='0';
			response:="00";			
			if uart_br = '1'  then			
				next_state:=rsetread;		
			end if;	
		elsif rstate = rsetread then
			debugid:=to_unsigned(2,4);	
			next_r:='1';
			next_state := rreceiving;
		elsif rstate = runsetread then
			debugid:=to_unsigned(3,4);	
			next_r:='0';
			next_state := rwaiting;
		elsif rstate = rreceiving then
			debugid:=to_unsigned(4,4);
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
			elsif uart_receive = p_cont then
				--cont
				response(0):='0';
				continue <= '1';
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
			next_state := runsetread;

		end if;		
		uart_rw(0)<=next_r;
		rstate<=next_state;
		rresponse<=response;
		bin_seg(7 downto 4)<=std_logic_vector(debugid);
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
				debugid:=to_unsigned(0,4);
				next_w:='0';
				response:="00";	
				next_state:=swaiting;
			elsif sstate = swaiting then
				debugid:=to_unsigned(1,4);	
				next_w:='0';
				response:="00";	
				if sending = '1'  then			
					next_state:=ssetwrite;
				end if;	
			elsif sstate = ssetwrite then
				debugid:=to_unsigned(2,4);	
				next_w:='1';
				next_state := ssending;
			elsif sstate = sunsetwrite then
				debugid:=to_unsigned(3,4);	
				next_w:='0';
				response:="10";			
				next_state := swaiting;
			elsif sstate = ssending then
				debugid:=to_unsigned(4,4);
				next_w:='0';		
				next_state := sunsetwrite;
			end if;
			uart_rw(1)<=next_w;
			sstate<=next_state;
			sresponse<=response;
			bin_seg(11 downto 8)<=std_logic_vector(debugid);
		end if;
	end process;

	motor_l_reset <= reset;
	motor_r_reset <= reset;
	count_reset <= reset;
end architecture b;