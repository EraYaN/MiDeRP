library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;

entity system_tb is
end entity system_tb;

architecture structural of system_tb is
	
	component system is
		port (
		   clk: in	std_logic; --kristal;
			reset: in std_logic; -- button;		
			
			sensor: in	std_logic_vector (2 downto 0); -- left = 0 middle = 1 right = 2
			servo: out	std_logic_vector (1 downto 0); -- left = 0 right = 1
			
			--uart
			rx: in std_logic;
			tx: out std_logic			
		);
	end component system;
	
	signal clk, rx, tx	: 	std_logic;
	signal reset		:	std_logic := '1' ;
	signal sensors	:	std_logic_vector (2 downto 0) := "000";
	signal servo : std_logic_vector(1 downto 0) := "00";
begin

	clk		<=	'1' after 0 ns,
				'0' after 10 ns when clk /= '0' else '1' after 10 ns;
	reset	<=	'0' after 40 ns,
				'1' after 460 us,
				'0' after 461 us;
	sensors <= "001" after 40 ms,
				"010" after 80 ms,
				"011" after 120 ms,
				"100" after 160 ms,
				"101" after 200 ms,
				"110" after 240 ms,
				"111" after 380 ms,
				"000" after 420 ms;
	rx <= '0';
	tx <= '0';
sys:	system port map (
	clk		=>	clk,
	reset	=>	reset,
	sensor	=>	sensors,
	servo => servo,
	rx => rx,
	tx => tx
);
					
end architecture structural;