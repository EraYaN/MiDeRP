library IEEE;
use IEEE.std_logic_1164.all;

entity threebitregister is
	port (	clk		: in	std_logic;
		reset : in std_logic;
		input : in std_logic_vector (2 downto 0);

		output : out std_logic_vector (2 downto 0)
	);
end entity threebitregister;

architecture b of threebitregister is
begin
	process(clk,reset)
	begin
		if(rising_edge(clk))then
			output <= input;
		end if;
		if(reset='1') then
			output <= "000";
		end if;
	end process;
end architecture b;