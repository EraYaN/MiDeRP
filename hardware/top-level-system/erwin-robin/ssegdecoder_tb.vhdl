library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;

entity ssegdecoder_tb is
end entity ssegdecoder_tb;

architecture structural of ssegdecoder_tb is
	
	component ssegdecoder is
		port(
			clk : in std_logic;
			bin_input : in STD_LOGIC_VECTOR (15 downto 0);
			dpoint : in std_logic_vector(3 downto 0);
			segments : out STD_LOGIC_VECTOR (7 downto 0);-- "DP,G,F,E,D,C,B,A"
			anodes : out std_logic_vector(3 downto 0);
			debug : out std_logic_vector(3 downto 0)
		);
	end component ssegdecoder;
	
	signal clk			: 	std_logic;	
	signal segments	:	STD_LOGIC_VECTOR (7 downto 0);
	signal anodes	:	STD_LOGIC_VECTOR (3 downto 0);
	signal bin_input	:	STD_LOGIC_VECTOR (15 downto 0);
	signal dpoint : std_logic_vector(3 downto 0);
	signal char : std_logic_vector(1 downto 0);
	signal next_char : std_logic_vector(1 downto 0);
	
begin

	clk		<=	'1' after 0 ns,
				'0' after 10 ns when clk /= '0' else '1' after 10 ns;
	dpoint	<=	"1011";
	bin_input	<=	x"0000",
					x"1234" after 80 ns,
					x"5678" after 160 ns,
					x"9ABC" after 320 ns,
					x"DEF0" after 400 ns;

l1:	ssegdecoder port map (
					clk		=>	clk,
					anodes => anodes,
					bin_input => bin_input,
					segments => segments,
					dpoint	=>	dpoint,
					debug(1 downto 0)=>next_char,
					debug(3 downto 2)=>char
					);
					
end architecture structural;