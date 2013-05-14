library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;
library std;
use std.textio.all;

entity motorcontrol is
	port (	clk		: in	std_logic;
		reset		: in	std_logic;
		speed		: in	signed (7 downto 0); -- van 100 tot -100
		count_in	: in	unsigned (19 downto 0);
		motor		: in	side; --0=left 1=right

		pwm		: out	std_logic
	);
end entity motorcontrol;

architecture b of motorcontrol is
	
begin
	process (clk, reset, motor)
		variable c, s, pulse : integer;
	begin
		c := To_integer (count_in);
		s := To_integer (speed);
		
		if (reset = '1' or s = 0) then
			pwm	<= '0';
		else
		
			if (motor = left) then
				pulse := 75000 + 250*s;
			else
				pulse := 75000 - 250*s;
			end if;
		
			if (c < pulse) then
				pwm <= '1';
			else 
				pwm <= '0';
			end if;
			
		end if;
	end process;
	
end architecture b;