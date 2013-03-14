library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;
library std;
use std.textio.all;

entity controller is
	port (	clk			: in	std_logic;
		reset			: in	std_logic;

		sensor_l		: in	std_logic;
		sensor_m		: in	std_logic;
		sensor_r		: in	std_logic;

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
	process (clk,sensor_l,sensor_m,sensor_r)
	begin
		motor_l_speed <= "00000000";
		motor_r_speed <= "00000000";		
		if(sensor_l='1' or sensor_m='1' or sensor_r='1')then
			motor_l_speed <= To_signed(100, 8);
			motor_r_speed <= To_signed(100, 8);
		
		end if;
	
	end process;
	motor_l_reset <= reset;
	motor_r_reset <= reset;
	count_reset <= reset;
end architecture b;