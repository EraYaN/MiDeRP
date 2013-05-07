library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;

entity threebitregister_tb is
end entity threebitregister_tb;

architecture structural of threebitregister_tb is
	
	component threebitregister is
		port (	clk		: in	std_logic;
		reset : in std_logic;
		input : in std_logic_vector (2 downto 0);

		output : out std_logic_vector (2 downto 0)
	);
	end component threebitregister;
	
	signal clk			: 	std_logic;
	signal reset		:	std_logic := '1' ;
	signal input, output	:	std_logic_vector (2 downto 0);
	
begin

	clk		<=	'1' after 0 ns,
				'0' after 10 ns when clk /= '0' else '1' after 10 ns;
	reset	<=	'0' after 40 ns,
				'1' after 600 ns,
				'0' after 640 ns;
	input <= "000" after 23 ns,
	"010" after 234 ns,
	"001" after 435 ns,
	"101" after 567 ns,
	"111" after 574 ns,
	"001" after 613 ns,
	"100" after 654 ns,
	"110" after 684 ns,
	"101" after 693 ns,
	"111" after 704 ns,
	"000" after 754 ns;

l1:	threebitregister port map (
					clk		=>	clk,
					reset	=>	reset,
					input	=>	input,
					output => output
					);
					
end architecture structural;