library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
-- Naar: SN74LS247 van commentaar van "imode" op "http://vhdlguru.blogspot.nl/2010/03/vhdl-code-for-bcd-to-7-segment-display.html"
entity ssegdecoder is
	port(
		bin_input : in STD_LOGIC_VECTOR (3 downto 0);
		dpoint : in std_logic;
		segments : out STD_LOGIC_VECTOR (7 downto 0) -- "DP,G,F,E,D,C,B,A"
	);
end ssegdecoder;

architecture Behavioral of ssegdecoder is
begin

--abcdefg
segments(6 downto 0) <= 
not "0111111" when (bin_input = "0000") else -- 0
not "0000110" when (bin_input = "0001") else -- 1
not "1011011" when (bin_input = "0010") else -- 2
not "1001111" when (bin_input = "0011") else -- 3
not "1100110" when (bin_input = "0100") else -- 4
not "1101101" when (bin_input = "0101") else -- 5
not "1111101" when (bin_input = "0110") else -- 6
not "0000111" when (bin_input = "0111") else -- 7
not "1111111" when (bin_input = "1000") else -- 8
not "1101111" when (bin_input = "1001") else -- 9
not "1110111" when (bin_input = "1010") else -- 10 (A)
not "1111100" when (bin_input = "1011") else -- 11 (b)
not "1100010" when (bin_input = "1100") else -- 12 (C)
not "1011110" when (bin_input = "1101") else -- 13 (d)
not "1111001" when (bin_input = "1110") else -- 14 (E)
not "1110001" when (bin_input = "1111") else -- 15 (F)
not "0000000";
segments(7)<=dpoint;

end Behavioral;