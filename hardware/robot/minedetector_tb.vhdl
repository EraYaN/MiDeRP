--mine detector test bench
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity minedetector_tb is
end entity minedetector_tb;

architecture s of minedetector_tb is

	component minedetector is
		port
		(
			clk, reset: in std_logic;
			wave_in: in std_logic;

			minedetected: out std_logic
		);
	end component;

	signal clk, reset, wave_in, minedetected : std_logic;

begin

	reset	<=	'1' after 0 ns,
				'0' after 20 ns;

	clk		<=	'1' after 0 ns,
				'0' after 10 ns when clk /= '0' else '1' after 10 ns;

	wave_in	<=	'1' after 0 ns,
				'0' after 0.8 ms when wave_in /= '0' else '1' after 0.8 ms;

	l1: minedetector port map (
		clk				=>	clk,
		reset			=>	reset,
		wave_in			=>	wave_in,
		minedetected	=>	minedetected
		);
		
end architecture s;