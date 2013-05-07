-- UART transmitter
-- incoming data byte din is serialized into outcoming bit stream tx
-- completion of each data frame is asserted by tx_done_tick

library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
entity uart_tx is
   port(
      clk, reset: in std_logic;
      tx_start: in std_logic; -- if '1' transmission starts
      s_tick: in std_logic; -- sampling tick from baud rate generator
      din: in std_logic_vector(7 downto 0); -- incoming data byte
      tx_done_tick: out std_logic; -- data frame completion tick 
      tx: out std_logic -- outcoming bit stream
   );
end uart_tx ;

architecture arch of uart_tx is
   constant DBIT: integer:=8; -- number of data bits
   constant SB_TICK: integer:=16;  -- numbner of stop bit ticks

   type state_type is (idle, start, data, stop);
   signal state_reg, state_next: state_type;
   signal s_reg, s_next: unsigned(3 downto 0); -- sampling tick counter
   signal n_reg, n_next: unsigned(2 downto 0); -- data bit counter
   signal b_reg, b_next: std_logic_vector(7 downto 0); -- data register
   signal tx_reg, tx_next: std_logic; -- bit stream flip-flop
begin
   -- FSMD state & data registers
   process(clk,reset)
   begin
      if reset='1' then
         state_reg <= idle;
         s_reg <= (others=>'0');
         n_reg <= (others=>'0');
         b_reg <= (others=>'0');
         tx_reg <= '1';
      elsif (clk'event and clk='1') then
         state_reg <= state_next;
         s_reg <= s_next;
         n_reg <= n_next;
         b_reg <= b_next;
         tx_reg <= tx_next;
      end if;
   end process;
   -- next-state logic & data path functional units/routing
   process(state_reg, s_reg, n_reg, b_reg, s_tick,
           tx_reg, tx_start, din)
   begin
      state_next <= state_reg; -- default values
      s_next <= s_reg;
      n_next <= n_reg;
      b_next <= b_reg;
      tx_next <= tx_reg ;
      tx_done_tick <= '0';
      case state_reg is
         when idle =>
            tx_next <= '1'; -- output in idle
            if tx_start='1' then -- transmission can start
               state_next <= start;
               s_next <= (others=>'0');
               b_next <= din;
            end if;
         when start =>
            tx_next <= '0'; -- output in START bit
            if (s_tick = '1') then
               if s_reg=15 then -- end of START bit reached
                  state_next <= data;
                  s_next <= (others=>'0');
                  n_next <= (others=>'0');
               else
                  s_next <= s_reg + 1;
               end if;
            end if;
         when data =>
            tx_next <= b_reg(0); -- data bit put on outputr
            if (s_tick = '1') then
               if s_reg=15 then -- end of DATA bit reached
                  s_next <= (others=>'0');
                  b_next <= '0' & b_reg(7 downto 1); -- data register shifted
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
            tx_next <= '1'; -- STOP bit put on output
            if (s_tick = '1') then
               if s_reg=(SB_TICK-1) then -- end of STOP bit reached
                  state_next <= idle;
                  tx_done_tick <= '1';
               else
                  s_next <= s_reg + 1;
               end if;
            end if;
      end case;
   end process;
   tx <= tx_reg;
end arch;
