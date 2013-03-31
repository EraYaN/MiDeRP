library ieee;
use ieee.std_logic_1164.all;

entity uart is
	port (
		clk, reset: in std_logic;
		rx: in std_logic; --input bit stream
		tx: out std_logic; --output bit stream
		sw: in std_logic_vector(7 downto 0); --byte to be sent
		led: out std_logic_vector(7 downto 0); --received byte
		write_data: in std_logic; --write to transmitter buffer 
		read_data: in std_logic; --read from receiver buffer 
		sseg: out std_logic; --_vector(7 downto 0); --seven segment LED display
		an: out std_logic_vector(3 downto 0) --anodes of seven segment LED display 
	);
end uart;

architecture structural of uart is
  --------------
  --COMPONENTS--
  --------------
  
  --BAUD_GENERATOR
  component baud_gen is
	   generic(
		  M: integer := 326 -- baud rate divisor M = 50M/(16*9600)
	  );
	   port(
		  clk, reset: in std_logic;
		  s_tick: out std_logic -- sampling tick
	   );
	end component baud_gen;
  
  --UART_RECEIVER
  component uart_rx is
   port(
      clk, reset: in std_logic;
      rx: in std_logic; -- icoming serial bit stream
      s_tick: in std_logic; -- sampling tick from baud rate generator
      rx_done_tick: out std_logic; -- data frame completion tick
      dout: out std_logic_vector(7 downto 0) -- data byte
   );
  end component uart_rx ;
  
  --BUFFER_REGISTER
  component buf_reg is
   port(
      clk, reset: in std_logic;
      clr_flag, set_flag: in std_logic; 
      din: in std_logic_vector(7 downto 0);
      dout: out std_logic_vector(7 downto 0);
      flag: out std_logic
   );
  end component buf_reg;
  
  --UART_TRANSMITTER
  component uart_tx is
   port(
      clk, reset: in std_logic;
      tx_start: in std_logic; -- if '1' transmission starts
      s_tick: in std_logic; -- sampling tick from baud rate generator
      din: in std_logic_vector(7 downto 0); -- incoming data byte
      tx_done_tick: out std_logic; -- data frame completion tick 
      tx: out std_logic -- outcoming bit stream
   );
  end component uart_tx ;
  
  -----------
  --SIGNALS--
  -----------
  signal s_tick, rx_done_tick, tx_done_tick, flag : std_logic;
  signal rx_dout, buf_dout : std_logic_vector ( 7 downto 0 );
  
  ---------------
  --PORTMAPPING--
  ---------------
begin
  
  baud_gen_unit: baud_gen port map ( 
    clk => clk,
    reset => reset,
    s_tick => s_tick  
  );
                  
  uart_rx_unit: uart_rx port map ( 
    clk => clk,
    reset => reset,
    rx => rx,
    s_tick => s_tick,
    rx_done_tick => rx_done_tick,
    dout => rx_dout
  );
  
  buf_reg_unit_1: buf_reg port map (
    clk => clk,
    reset => reset,
    clr_flag => read_data,
    set_flag => rx_done_tick,
    din => rx_dout,
    dout => led,
    flag => sseg
  );
  
  buf_reg_unit_2: buf_reg port map (
    clk => clk,
    reset => reset,
    clr_flag => tx_done_tick,
    set_flag => write_data,
    din => sw,
    dout => buf_dout,
    flag => flag
  );
  
  uart_tx_unit: uart_tx port map (  
    clk => clk,
    reset => reset,
    tx_start => flag,
    s_tick => s_tick,
    din => buf_dout,
    tx_done_tick => tx_done_tick,
    tx => tx
  );
  
end architecture;
  
  
  