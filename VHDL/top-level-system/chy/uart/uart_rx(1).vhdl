-- UART receiver
-- incoming serial bit stream rx is grouped in 8-bit data byte dout
-- completion of each data frame is asserted by rx_done_tick

library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
entity uart_rx is
   port(
      clk, reset: in std_logic;
      rx: in std_logic; -- icoming serial bit stream
      s_tick: in std_logic; -- sampling tick from baud rate generator
      rx_done_tick: out std_logic; -- data frame completion tick
      dout: out std_logic_vector(7 downto 0) -- data byte
   );
end uart_rx ;

architecture arch of uart_rx is
   constant DBIT: integer:=8; -- number of data bits
   constant SB_TICK: integer:=16;  -- numbner of stop bit ticks

   type state_type is (idle, start, data, stop);
   signal state_reg, state_next: state_type;
   signal s_reg, s_next: unsigned(3 downto 0); -- sampling tick counter
   signal n_reg, n_next: unsigned(2 downto 0); -- data bit counter
   signal b_reg, b_next: std_logic_vector(7 downto 0); -- data register
begin
   -- FSMD state & data registers
   process(clk, reset)
   begin
      if reset='1' then
         state_reg <= idle;
         s_reg <= (others=>'0');
         n_reg <= (others=>'0');
         b_reg <= (others=>'0');
      elsif (clk'event and clk='1') then
         state_reg <= state_next;
         s_reg <= s_next;
         n_reg <= n_next;
         b_reg <= b_next;
      end if;
   end process;
   -- next-state logic & data path functional units/routing
   process(state_reg, s_reg, n_reg, b_reg, s_tick, rx)
   begin
      state_next <= state_reg; -- default values
      s_next <= s_reg;
      n_next <= n_reg;
      b_next <= b_reg;
      rx_done_tick <='0';
      case state_reg is
         when idle =>
            if rx='0' then -- falling edge of the START bit detected
               state_next <= start;
               s_next <= (others=>'0');
            end if;
         when start =>
            if (s_tick = '1') then
               if s_reg=7 then -- midpoint of the START bit reached
                  state_next <= data;
                  s_next <= (others=>'0');
                  n_next <= (others=>'0');
               else
                  s_next <= s_reg + 1;
               end if;
            end if;
         when data =>
            if (s_tick = '1') then
               if s_reg=15 then -- midpoint of DATA bit reached
                  s_next <= (others=>'0');
                  b_next <= rx & b_reg(7 downto 1); -- sampled DATA bit shifted into data register
                  if n_reg=(DBIT-1) then -- last DATA bit reached
                     state_next <= stop ;
                  else
                     n_next <= n_reg + 1;
                  end if;
               else
                  s_next <= s_reg + 1;
               end if;
            end if;
         when stop =>
            if (s_tick = '1') then
               if s_reg=(SB_TICK-1) then -- midpoint of STOP bit reached
                  state_next <= idle;
                  rx_done_tick <='1';
               else
                  s_next <= s_reg + 1;
               end if;
            end if;
      end case;
   end process;
   dout <= b_reg;
end arch;