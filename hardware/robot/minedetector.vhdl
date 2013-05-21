--mine detector
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity minedetector is
   port
   (
      clk, reset: in std_logic;
      wave_in: in std_logic;
	  
	  minedetected: out std_logic
   );
end minedetector ;

architecture arch of minedetector is
   constant threshold : integer := 37750; --amount of ticks per pulse to trigger detector
   signal count : integer;
   signal prev, counting : std_logic;
begin

	process(clk)
		variable next_count: integer;
		variable next_prev, next_counting, next_minedetected : std_logic;
	begin
		if rising_edge(clk) then
			next_minedetected := '0';
			if reset = '1' then
				next_count := 0;
				next_prev := '0';
				next_minedetected := '0';		
			elsif wave_in = '1' and prev = '0' then
				next_counting := '1'; --pulse detected
			elsif wave_in = '0' and prev = '1' then
				next_counting := '0';				
				if count > threshold then
					next_minedetected := '1';
				end if;				
			else
				next_prev := wave_in;
			end if;			
			if counting = '1' then			
				next_count := count + 1;
				else
				next_count:=0;
			end if;
		end if;

		
		
		prev <= next_prev;
		counting <= next_counting;
		count <= next_count;
		minedetected <= next_minedetected;
	end process;
   
end arch;
