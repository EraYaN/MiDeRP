library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
-- Naar: SN74LS247 van commentaar van "imode" op "http://vhdlguru.blogspot.nl/2010/03/vhdl-code-for-bcd-to-7-segment-display.html"
entity ssegdecoder is
	port(
		clk : in std_logic;
		reset : in std_logic;
		bin_input : in STD_LOGIC_VECTOR (15 downto 0);
		dpoint : in std_logic_vector(3 downto 0);
		segments : out STD_LOGIC_VECTOR (7 downto 0);-- "DP,G,F,E,D,C,B,A"
		anodes : out std_logic_vector(3 downto 0)		
	);
end ssegdecoder;

architecture Behavioral of ssegdecoder is
signal char : std_logic_vector(1 downto 0):="00";
begin
process(clk, reset) is
	variable bin : std_logic_vector(3 downto 0):=x"F";
	variable next_char : std_logic_vector(1 downto 0);
begin
	--default values
	segments <= "11111111";
	anodes <= "1111";

	if reset = '1' then
		char <= "00";	
	else
		--abcdefg
		if char = "00" then
			next_char:="01";
			bin := bin_input(3 downto 0);
			anodes <= "1110";
			segments(7)<=not dpoint(0);
		elsif char = "01" then
			next_char:="10";
			bin := bin_input(7 downto 4);
			anodes <= "1101";
			segments(7)<=not dpoint(1);
		elsif char = "10" then
			next_char:="11";
			bin := bin_input(11 downto 8);
			anodes <= "1011";
			segments(7)<=not dpoint(2);
		elsif char = "11" then
			next_char:="00";
			bin := bin_input(15 downto 12);
			anodes <= "0111";
			segments(7)<=not dpoint(3);
		else
			next_char:="01";
			bin := bin_input(3 downto 0);
			anodes <= "1110";
			segments(7)<=not dpoint(0);
		end if;
		case bin is
			when "0000"=> segments(6 downto 0) <= not "0111111" ; -- 0
			when "0001"=> segments(6 downto 0) <= not "0000110" ; -- 1
			when "0010"=> segments(6 downto 0) <= not "1011011" ; -- 2
			when "0011"=> segments(6 downto 0) <= not "1001111"; -- 3
			when "0100"=> segments(6 downto 0) <= not "1100110" ; -- 4
			when "0101"=> segments(6 downto 0) <= not "1101101"; -- 5
			when "0110"=> segments(6 downto 0) <= not "1111101"; -- 6
			when "0111"=> segments(6 downto 0) <= not "0000111"; -- 7
			when "1000"=> segments(6 downto 0) <= not "1111111"; -- 8
			when "1001"=> segments(6 downto 0) <= not "1101111"; -- 9
			when "1010"=> segments(6 downto 0) <= not "1110111"; -- 10 (A)
			when "1011"=> segments(6 downto 0) <= not "1111100"; -- 11 (b)
			when "1100"=> segments(6 downto 0) <= not "1100010"; -- 12 (C)
			when "1101"=> segments(6 downto 0) <= not "1011110"; -- 13 (d)
			when "1110"=> segments(6 downto 0) <= not "1111001"; -- 14 (E)
			when "1111"=> segments(6 downto 0) <= not "1110001"; -- 15 (F)
			when others => segments(6 downto 0) <= not "0000000";
		end case;
		char<=next_char;
	end if;
end process;
end Behavioral;