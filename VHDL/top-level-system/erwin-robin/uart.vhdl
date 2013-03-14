library IEEE;
use IEEE.std_logic_1164.all;

entity uart is
	port (
		clk, reset: in std_logic;
		rx: in std_logic; --input bit stream
		tx: out std_logic; --output bit stream
		sw: in std_logic_vector(7 downto 0); --byte to be sent
		led: out std_logic_vector(7 downto 0); --received byte
		write_data: in std_logic; --write to transmitter buffer 
		read_data: in std_logic; --read from receiver buffer 
		sseg: out std_logic; --seven segment LED display
		an: out std_logic_vector(3 downto 0) --anodes of seven segment LED display 
	);
end uart;

architecture s of uart is
	
	component baud_gen is
	   generic(
		  M: integer := 326 -- baud rate divisor M = 50M/(16*9600)
	  );
	   port(
		  clk, reset: in std_logic;
		  s_tick: out std_logic -- sampling tick
	   );
	end component baud_gen;
	
	component buf_reg is
	   port(
		  clk, reset: in std_logic;
		  clr_flag, set_flag: in std_logic; 
		  din: in std_logic_vector(7 downto 0);
		  dout: out std_logic_vector(7 downto 0);
		  flag: out std_logic
	   );
	end component buf_reg;
	
	component uart_rx is
	   port(
		  clk, reset: in std_logic;
		  rx: in std_logic; -- icoming serial bit stream
		  s_tick: in std_logic; -- sampling tick from baud rate generator
		  rx_done_tick: out std_logic; -- data frame completion tick
		  dout: out std_logic_vector(7 downto 0) -- data byte
	   );
	end component uart_rx ;
	
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
	
	signal s_tick, rx_done_tick, tx_done_tick, tx_start : std_logic;
	signal received, transmit : std_logic_vector(7 downto 0);
	
begin

	l1: baud_gen port map (
		clk	=>	clk,
		reset	=>	reset,
		s_tick	=>	s_tick
		);
	l2:	buf_reg	port map (
		clk	=>	clk,
		reset	=>	reset,
		clr_flag	=>	read_data,
		set_flag	=>	rx_done_tick,
		din	=>	received,
		dout	=>	led,
		flag	=>	sseg
		);
	l3: buf_reg port map (
		clk	=>	clk,
		reset	=>	reset,
		clr_flag	=>	tx_done_tick,
		set_flag	=>	write_data,
		din	=>	sw,
		dout	=>	transmit,
		flag	=>	tx_start
		);
	l4: uart_rx port map (
		clk	=>	clk,
		reset	=>	reset,
		rx	=>	rx,
		s_tick	=>	s_tick,
		rx_done_tick	=>	rx_done_tick,
		dout	=>	received
		);
	l5: uart_tx port map (
		clk	=>	clk,
		reset	=>	reset,
		tx_start	=>	tx_start,
		s_tick	=>	s_tick,
		din	=>	transmit,
		tx_done_tick	=>	tx_done_tick,
		tx	=>	tx
		);

end architecture;	
	
	
	