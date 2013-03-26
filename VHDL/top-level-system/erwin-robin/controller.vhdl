library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;
library std;
use std.textio.all;

entity controller is
	port (	clk			: in	std_logic;
		reset			: in	std_logic;

		sensor		: in	std_logic_vector(2 downto 0);		

		count_in		: in	unsigned (19 downto 0);
		count_reset		: out	std_logic;

		motor_l_reset		: out	std_logic;
		motor_l_speed		: out	signed (7 downto 0);

		motor_r_reset		: out	std_logic;
		motor_r_speed		: out	signed (7 downto 0);
		
		bin_seg					: out std_logic_vector (3 downto 0);
		dpoint_seg					: out std_logic;
		led					: out std_logic_vector (7 downto 0);
		an					: out std_logic_vector (3 downto 0);
		
		uart_send				: out std_logic_vector(7 downto 0);
		uart_receive			: in std_logic_vector(7 downto 0);
		uart_rw					: out std_logic_vector(1 downto 0); -- 0 = read, 1 = write
		uart_br 				: in std_logic
	);
end entity controller;
architecture b of controller is	
	type sys_state is (followline, processnextturn, leftturn, rightturn, waitforinput);
	signal state : sys_state := followline;
	signal nextturn : unsigned(1 downto 0) := to_unsigned(1,4); -- 0 = left, 1 = forward, 2 = right, 3 = stop
	begin
	an<="1110";
	led(1 downto 0)<=std_logic_vector(nextturn);
	led(7 downto 2)<="000000";
	dpoint_seg<='0';
	process (clk) is
	variable next_state : sys_state;
	variable debugid : unsigned ( 3 downto 0);
	begin
	if rising_edge(clk) then
		next_state:=state;
		motor_l_speed <= "00000000";
		motor_r_speed <= "00000000";
		if reset = '1' then
			next_state:=followline;
		elsif state = followline then
			debugid:=to_unsigned(1,4);
			uart_rw(0)<='1';
			uart_rw(1)<='0';
			--follow line
			case sensor is
			  when "000" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8); next_state:=processnextturn;
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
			uart_rw(0)<='1';
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
				next_state:=waitforinput; --stop (wait for input)
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
		elsif state = waitforinput then
			debugid:=to_unsigned(5,4);
			--wait for input
			uart_send <= "01100110";
			uart_rw(1)<='1';
			
			--uart_rw(1)<='0';
			uart_rw(0)<='1';
			
			motor_l_speed <= to_signed(0,8);
			motor_r_speed <= to_signed(0,8);
			--wait until( uart_br'event and uart_br='1');			
			if uart_br = '1' then
				next_state := processnextturn;
				uart_rw(0)<='0';
				uart_rw(1)<='0';
			end if;			
		else
			debugid:=to_unsigned(15,4);
			--stop
			motor_l_speed <= to_signed(0,8);
			motor_r_speed <= to_signed(0,8);
		end if;		   
		state<=next_state;
		bin_seg<=std_logic_vector(debugid);
	end if;
	end process;
	process (uart_br)
	--variable next_state : sys_state;
	variable r : unsigned (7 downto 0);
	begin
		if rising_edge(uart_br) then			
			r:=unsigned(uart_receive);		
			if r = 76 then
				--left
				nextturn<=to_unsigned(0,2);
			elsif r = 82 then
				--right
				nextturn<=to_unsigned(2,2);
			elsif r = 70 then
				--forward
				nextturn<=to_unsigned(1,2);
			elsif r = 83 then
				--stop
				nextturn<=to_unsigned(3,2);
			else
				--discard				
			end if;			
		end if;
	end process;
	motor_l_reset <= reset;
	motor_r_reset <= reset;
	count_reset <= reset;
end architecture b;