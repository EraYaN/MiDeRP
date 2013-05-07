-- Buffer register with flag flip-flop
-- used as interface buffer between UART and main system

library ieee;
use ieee.std_logic_1164.all;
entity buf_reg is
   port(
      clk, reset: in std_logic;
      clr_flag, set_flag: in std_logic; 
      din: in std_logic_vector(7 downto 0);
      dout: out std_logic_vector(7 downto 0);
      flag: out std_logic
   );
end buf_reg;

architecture arch of buf_reg is
   signal b_reg, b_next: std_logic_vector(7 downto 0); -- data register
   signal flag_reg, flag_next: std_logic; -- flag FF
begin
   -- FF & register
   process(clk,reset)
   begin
      if reset='1' then
         b_reg <= (others=>'0');
         flag_reg <= '0';
      elsif (clk'event and clk='1') then
         b_reg <= b_next;
         flag_reg <= flag_next;
      end if;
   end process;
   -- next-state logic
   process(b_reg, flag_reg, set_flag, clr_flag, din)
   begin
      b_next <= b_reg;
      flag_next <= flag_reg;
      if (set_flag='1') then 
         b_next <= din;
         flag_next <= '1'; -- data will be written into register
      elsif (clr_flag='1') then
         flag_next <= '0';
      end if;
   end process;
   -- output logic
   dout <= b_reg;
   flag <= flag_reg;
end arch;
