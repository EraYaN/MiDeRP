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
	--protocol
	constant p_forward	: byte := x"46"; -- 'F'
	constant p_stop		: byte := x"53"; -- 'S'
	constant p_left		: byte := x"4C"; -- 'L'
	constant p_right	: byte := x"52"; -- 'R'
	constant p_turn		: byte := x"54"; -- 'T'
	constant p_ack		: byte := x"06"; -- ACK
	constant p_nak		: byte := x"15"; -- NAK
	constant p_enq		: byte := x"05"; -- ENQ
	constant p_mine		: byte := x"07"; -- BEL
	constant p_done		: byte := x"04"; -- EOT
	constant p_unknown	: byte := x"00"; -- NULL
	
	type sys_state is (followline, processnextturn, leftturn, rightturn, callforinput, waitforinput, sendok, sendfail);
	type sender_state is (swaiting, ssending, ssetwrite, sunsetwrite);
	type receiver_state is (rwaiting, rreceiving, rsetread, runsetread);
	signal state : sys_state := followline;
	signal sstate : sender_state := swaiting;
	signal rstate : receiver_state := rwaiting;
	signal uart_rw : std_logic_vector(1 downto 0);
	signal nextturn : unsigned(1 downto 0) := to_unsigned(1,4); -- 0 = left, 1 = forward, 2 = right, 3 = stop	
	signal sending : std_logic := '0'; -- internal
	signal rresponse, sresponse : std_logic_vector (1 downto 0) := "00";
	
begin
	
	bin_seg(15 downto 12)<="1111";
	led(1 downto 0)<=std_logic_vector(nextturn);
	led(3 downto 2)<=rresponse;
	led(5 downto 4)<=sresponse;
	led(6)<=sending;
	led(7)<=uart_rw(1);
	dpoint_seg(0)<=uart_br;
	dpoint_seg(3 downto 1)<="101";
	uart_rw_out<=uart_rw;
	
	process (clk) is
		variable next_state : sys_state;
		variable debugid : unsigned ( 3 downto 0);
		variable next_sending :std_logic;
	begin
		--default values
		motor_l_speed <= "00000000";
		motor_r_speed <= "00000000";
		bin_seg(11 downto 0) = "111111111111";
		uart_send = p_unknown;
	
		if rising_edge(clk) then
			next_state:=state;
			next_sending:=sending;
			if reset = '1' then
				next_state:=followline;
			elsif state = followline then
				debugid:=to_unsigned(1,4);			
				--follow line
				case sensor is
					when "000" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8); next_state:=callforinput;
					when "001" => motor_l_speed <= to_signed(0,8); motor_r_speed <= to_signed(100,8);
					when "010" => motor_l_speed <= to_signed(50,8); motor_r_speed <= to_signed(50,8);
					when "011" => motor_l_speed <= to_signed(-50,8); motor_r_speed <= to_signed(100,8);
					when "100" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(0,8);
					when "101" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8);
					when "110" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(-50,8);
					when "111" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8);
					when others => motor_l_speed <= to_signed(0,8); motor_r_speed <= to_signed(0,8);
			   end case;
			elsif state = processnextturn then
				debugid:=to_unsigned(2,4);
				motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8);
				--uart_rw(0)<='1';
				--nextturn			
				if nextturn = 0 then
					next_state:=leftturn; --left
					--nextturn <= to_unsigned(3,2);
				elsif nextturn = 1 then
					next_state:=followline; --forward (line)
					--nextturn <= to_unsigned(3,2);
				elsif nextturn = 2 then
					next_state:=rightturn; --right
					--nextturn <= to_unsigned(3,2);
				elsif nextturn = 3 then
					next_state:=callforinput; --stop (wait for input)
				end if;
			elsif state = leftturn then
				debugid:=to_unsigned(3,4);
				--left
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(100,8);
				case sensor is			  			  
				  when "101" => next_state:=followline;
				  when others => --nothing
			   end case;
			elsif state = rightturn then
				debugid:=to_unsigned(4,4);
				--right
				motor_l_speed <= to_signed(100,8);
				motor_r_speed <= to_signed(0,8);
				case sensor is			  			  
				  when "101" => next_state:=followline;
				  when others => --nothing
			   end case;
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
			elsif state = waitforinput then
				debugid:=to_unsigned(6,4);			
				motor_l_speed <= to_signed(0,8);
				motor_r_speed <= to_signed(0,8);					
				if rresponse(1) = '1'  then					
					if rresponse(0) = '0' then
						next_state:=sendok;
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
			sending<=next_sending;
			bin_seg(3 downto 0)<=std_logic_vector(debugid);
		end if;
	end process;
	
	--receiver
<<<<<<< HEAD
	process (clk, rstate, uart_rw, uart_br, uart_receive)
	variable next_state : receiver_state;
	variable next_r : std_logic;
	variable debugid : unsigned ( 3 downto 0);
	variable response : std_logic_vector (1 downto 0);	
=======
	process (clk)
		variable next_state : receiver_state;
		variable next_r : std_logic;
		variable debugid : unsigned ( 3 downto 0);
		variable response : std_logic_vector (1 downto 0);	
>>>>>>> 9c83eb65def44f9596900dfb71229f1b7ae49dea
	begin
		next_state:=rstate;
		response:="00";	
		next_r:=uart_rw(0);	
		if rstate = rwaiting then
		debugid:=to_unsigned(1,4);	
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
			response(1):='1';
			if uart_receive = p_left then
				--left
				response(0):='0';
				nextturn<=to_unsigned(0,2);
			elsif uart_receive = p_right then
				--right
				response(0):='0';
				nextturn<=to_unsigned(2,2);
			elsif uart_receive = p_forward then
				--forward
				response(0):='0';
				nextturn<=to_unsigned(1,2);
			elsif uart_receive = p_stop then
				--stop
				response(0):='0';
				nextturn<=to_unsigned(3,2);
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
	end process;
	
	--sender
	process (clk, uart_rw, sstate, sending)
	variable next_state : sender_state;
	variable next_w : std_logic;
	variable debugid : unsigned ( 3 downto 0);
	variable response : std_logic_vector (1 downto 0);		
	begin			
		next_w:=uart_rw(1);
		if sstate = swaiting then
		debugid:=to_unsigned(1,4);	
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
			next_state := sunsetwrite;
		end if;
		uart_rw(1)<=next_w;
		sstate<=next_state;
		sresponse<=response;
		bin_seg(11 downto 8)<=std_logic_vector(debugid);
	end process;
	motor_l_reset <= reset;
	motor_r_reset <= reset;
	count_reset <= reset;
end architecture b;