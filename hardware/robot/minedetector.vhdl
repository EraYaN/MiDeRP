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
end minedetector;

architecture arch of minedetector is
   constant threshold : integer := 38000; --amount of ticks per pulse to trigger detector
   signal count : integer := 0;
   signal count2 : integer := 0;
   signal prev, counting : std_logic;
begin

	process(clk)
		variable next_count,next_count2: integer;
		variable next_prev, next_counting, next_minedetected : std_logic;
	begin
		if rising_edge(clk) then
			next_minedetected := '0';
			
			if reset = '1' then
				next_count := 0;
				next_count2 := 0;
				next_prev := '0';
				next_counting := '0';
				next_minedetected := '0';		
			elsif wave_in = '1' and prev = '0' then
				next_counting := '1'; --pulse detected
				next_prev := wave_in;
				next_count2:=count2;
			elsif wave_in = '0' and prev = '1' then
				next_counting := '0';				
				if count > threshold then
					--next_count2 := count2+1;
					if count2 >= 30 then
						next_minedetected := '1';
						next_count2 := 0;
					else
						next_count2 := count2+1;
					end if;
				else
					next_minedetected := '0';
					next_count2 := count2-2;
					if next_count2 <0 then
						next_count2 := 0;
						end if;
				end if;
				next_prev := wave_in;			
			else
				next_prev := wave_in;
				next_count2 := count2;
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
		count2 <= next_count2;
		minedetected <= next_minedetected;
	end process;

end arch;
