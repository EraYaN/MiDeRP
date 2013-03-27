-- Baud rate generator
-- based on mod-M counter
-- a sampling tick is generated when counter has maximumm value

library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity baud_gen is
   generic(
      M: integer := 326; -- baud rate divisor M = 50M/(16*9600)
  );
   port(
      clk, reset: in std_logic;
      s_tick: out std_logic; -- sampling tick
   );
end baud_gen;

-- 9 bit register to allow divisor M = 326 (M < 512)
architecture arch of baud_gen is
   signal r_reg, r_next: unsigned(8 downto 0); 

begin
   -- register
   process(clk, reset)
   begin
      if (reset='1') then
         r_reg <= (others=>'0');
      elsif (clk'event and clk='1') then
         r_reg <= r_next;
      end if;
   end process;
   -- next-state logic
   r_next <= (others=>'0') when r_reg=(M-1) else
             r_reg + 1;
   -- output logic
   s_tick <= '1' when r_reg=(M-1) else '0';
end arch;