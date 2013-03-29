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
	signal	count, new_count	: unsigned (19 downto 0):= "00000000000000000000";

begin
	-- Dit process genereert het register
	process (clk)
	begin
		if (rising_edge (clk)) then
			if (reset = '1') then
				count	<= (others => '0');	-- zet op 0 bij reset
			else
				count	<= count +1;
			end if;
		end if;
	end process;

	count_out	<= std_logic_vector (count);

end architecture behavioural;