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
		motor_r_speed		: out	signed (7 downto 0)
	);
end entity controller;
architecture b of controller is

	begin
	process (clk,sensor)
	begin
		motor_l_speed <= "00000000";
		motor_r_speed <= "00000000";	
		case sensor is
          when "000" => motor_l_speed <= to_signed(30,8); motor_r_speed <= to_signed(30,8);
          when "001" => motor_l_speed <= to_signed(0,8); motor_r_speed <= to_signed(100,8);
          --when "010" => motor_l_speed <= to_signed(,8); motor_r_speed <= to_signed(,8);
          when "011" => motor_l_speed <= to_signed(-50,8); motor_r_speed <= to_signed(100,8);
          when "100" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(0,8);
          when "101" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8);
          when "110" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(-50,8);
          when "111" => motor_l_speed <= to_signed(100,8); motor_r_speed <= to_signed(100,8);
          when others => motor_l_speed <= to_signed(0,8); motor_r_speed <= to_signed(0,8);
       end case;		
	
	end process;
	motor_l_reset <= reset;
	motor_r_reset <= reset;
	count_reset <= reset;
end architecture b;