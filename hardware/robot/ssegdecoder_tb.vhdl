library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;

entity ssegdecoder_tb is
end entity ssegdecoder_tb;

architecture structural of ssegdecoder_tb is
	
	component ssegdecoder is
		port(
			clk : in std_logic;
			reset : in std_logic;
			bin_input : in STD_LOGIC_VECTOR (15 downto 0);
			dpoint : in std_logic_vector(3 downto 0);
			segments : out STD_LOGIC_VECTOR (7 downto 0);-- "DP,G,F,E,D,C,B,A"
			anodes : out std_logic_vector(3 downto 0)		
		);
	end component ssegdecoder;
	
	signal clk,reset			: 	std_logic;	
	signal segments	:	STD_LOGIC_VECTOR (7 downto 0);
	signal anodes	:	STD_LOGIC_VECTOR (3 downto 0);
	signal bin_input	:	STD_LOGIC_VECTOR (15 downto 0);
	signal dpoint : std_logic_vector(3 downto 0);
	
begin
	reset <= '1',
	'0' after 40 ns;
	clk		<=	'1' after 0 ns,
				'0' after 10 ns when clk /= '0' else '1' after 10 ns;
	dpoint	<=	"1011";
	bin_input	<=	x"0000",
					x"1234" after 120 ns,
					x"5678" after 200 ns,
					x"9ABC" after 360 ns,
					x"DEF0" after 440 ns;

l1:	ssegdecoder port map (
		clk		=>	clk,
		reset => reset,
		anodes => anodes,
		bin_input => bin_input,
		segments => segments,
		dpoint	=>	dpoint					
	);
					
end architecture structural;