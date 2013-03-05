library IEEE;
use IEEE.std_logic_1164.all;

entity motorcontrol is
	port (	clk		: in	std_logic;
		reset		: in	std_logic;
		direction	: in	std_logic;
		count_in	: in	std_logic_vector (19 downto 0);

		pwm		: out	std_logic
	);
end entity motorcontrol;
