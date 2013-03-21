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
		
		uart_send				: out std_logic_vector(7 downto 0);
		uart_receive			: in std_logic_vector(7 downto 0);
		uart_rw					: out std_logic_vector(1 downto 0); -- 0 = read, 1 = write
		uart_br 				: in std_logic
	);
end entity controller;
architecture b of controller is	
	type state is(followline, nextturn, leftturn, rightturn, waitforinput)
	signal state : state);
	signal nextturn : unsigned(1 downto 0); -- 0 = left, 1 = forward, 2 = right, 3 = stop
	begin
	process (clk)
	variable next_state : state;
	begin
	if rising_edge(clk) then
		next_state:=state;
		motor_l_speed <= "00000000";
		motor_r_speed <= "00000000";
		if state = followline then
			--follow line
			case sensor is
			  when "000" => motor_l_speed <= to_signed(0,8); motor_r_speed <= to_signed(0,8); next_state:=nextturn;
			  when "001" => motor_l_speed <= to_signed(0,8); motor_r_speed <= to_signed(100,8);
			  --when "010" => motor_l_speed <= to_signed(,8); motor_r_speed <= to_signed(,8);
			  when "011" => motor_l_speed <= to_signed(-50,8); motor_r_speed <= to_signed(100,8);
			  when "100" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(0,8);
			  when "101" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8);
			  when "110" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(-50,8);
			  when "111" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8);
			  when others => motor_l_speed <= to_signed(0,8); motor_r_speed <= to_signed(0,8);
		   end case;
		elsif state = nextturn then
			--nextturn			
			if nextturn = 0 then
				next_state:=leftturn; --left
			elsif nextturn = 1 then
				next_state:=followline; --forward (line)
			elsif nextturn = 2 then
				next_state:=rightturn; --right
			elsif nextturn = 3 then
				next_state:=waitforinput; --stop (wait for input)
			end if;
		elsif state = leftturn then
			--left
			motor_l_speed <= to_signed(-100,8);
			motor_l_speed <= to_signed(100,8);
		elsif state = 3 then
			--right
			motor_l_speed <= to_signed(100,8);
			motor_l_speed <= to_signed(-100,8);
		elsif state = 4 then
			--wait for input
			motor_l_speed <= to_signed(0,8);
			motor_r_speed <= to_signed(0,8);
			--wait until( uart_br'event and uart_br='1');
			uart_rw(0);
			if uart_br = '1' then
				next_state := 1;
			end if;			
		else
			--stop
			motor_l_speed <= to_signed(0,8); motor_r_speed <= to_signed(0,8);
		end if;		   
		state<=next_state;
	end if;
	end process;
	process (uart_br)
	variable s : integer :=0;
	variable r : unsigned (7 downto 0);
	begin
		if rising_edge(uart_br) then
			s:=to_integer(state);	
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
			state<=to_unsigned(s,8);
		end if;
	end process;
	motor_l_reset <= reset;
	motor_r_reset <= reset;
	count_reset <= reset;
end architecture b;