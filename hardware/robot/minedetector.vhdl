--mine detector
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity minedetector is
   port
   (
      clk, reset: in std_logic;
      wave_in: in std_logic_vector(3 downto 0);
	  
	  minedetected: out std_logic
   );
end minedetector ;

architecture arch of minedetector is
   constant threshold : integer := 1337; --amount of ticks per pulse to trigger detector
   signal count : integer;
   signal prev, counting : std_logic;
begin

	process(clk)
		variable next_count: integer;
		variable next_prev, next_counting : std_logic;
	begin
		if rising_edge(clk) then
			if reset = '1' then
				next_count := 0;
				minedetected <= '0';		
			elsif wave_in(0) = '1' and prev = '0' then
				next_counting := '1'; --pulse detected
			elsif wave_in(0) = '0' and prev = '1' then
				next_counting := '0';
				
				if count > threshold then
					minedetected <= '1';
				end if;
				
				next_count := 0;
			end if;		
			next_count := next_count + 1;
		end if;

		next_prev = wave_in(0);
		
		prev <= next_prev;
		counting <= next_counting;
		count <= next_count;
		
	end process;
   
end arch;
