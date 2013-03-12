library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;
library std;
use std.textio.all;

entity motorcontrol_tb is
end entity motorcontrol_tb;

architecture s of motorcontrol_tb is

	component counter is
		port (	
			clk			: in	std_logic;
			reset		: in	std_logic;

			count_out	: out	std_logic_vector (19 downto 0)
		);
	end component counter;
	
	component motorcontrol is
		port (	
			clk			: in	std_logic;
			reset		: in	std_logic;
			speed		: in	signed (7 downto 0); -- van 100 tot -100
			count_in	: in	std_logic_vector (19 downto 0);
			motor		: in	side;

			pwm		: out	std_logic
		);
	end component motorcontrol;
	
	signal clk, reset, pwm : std_logic;
	signal count : std_logic_vector (19 downto 0);
	signal motor : side;
	signal speed : signed (7 downto 0);

begin

	clk		<=	'1' after 0 ns,
				'0' after 10 ns when clk /= '0' else '1' after 10 ns;
	reset	<=	'1' after 0 ns,
				'0' after 40 ns,
				'1' after 300 ms,
				'0' after 301 ms;
	speed	<=	to_signed(0, 8) after 0 ns,
				to_signed(50, 8) after 40 ms,
				to_signed(100, 8) after 80 ms,
				to_signed(-100, 8) after 120 ms,
				to_signed(-50, 8) after 160 ms,
				to_signed(0, 8) after 200 ms,
				to_signed(50, 8) after 250 ms,
				to_signed(50, 8) after 340 ms,
				to_signed(100, 8) after 380 ms,
				to_signed(-100, 8) after 420 ms,
				to_signed(-50, 8) after 460 ms,
				to_signed(0, 8) after 500 ms;
	motor	<=	left after 0 ns,
				right after 300 ms;
	
	l1: counter port map (
		clk			=>	clk,
		reset		=>	reset,
		count_out	=>	count
		);
	l2: motorcontrol port map (
		clk			=>	clk,
		reset		=> 	reset,
		speed		=>	speed,
		count_in	=>	count,
		motor		=>	motor,
		pwm			=>	pwm
		);
		
end architecture s;