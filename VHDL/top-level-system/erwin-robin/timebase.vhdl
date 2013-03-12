library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;

entity counter is
	port (	clk		: in	std_logic;
		reset		: in	std_logic;

		count_out	: out	std_logic_vector (19 downto 0)
	);
end entity counter;

architecture behavioural of counter is

	signal	count, new_count	: unsigned (19 downto 0);

begin
	-- Dit process genereert het register
	process (clk)
	begin
		if (rising_edge (clk)) then
			if (reset = '1') then
				count	<= (others => '0');	-- zet op 0 bij reset
			else
				count	<= new_count;
			end if;
		end if;
	end process;

	-- Dit process berekent de nieuwe count-waarde
	process (count)
	begin
		new_count	<= count + 1;		-- alleen optellen als enable = '1'
	end process;

	-- Met deze regel wordt de count-waarde naar buiten gebracht
	count_out	<= std_logic_vector (count);

end architecture behavioural;
