library IEEE;
use IEEE.std_logic_1164.all;

entity test_controller_tb is
end entity test_controller_tb;

architecture structural of test_controller_tb is
  --TEST_CONTROLLER
  component test_controller is
    port ( clk       : in std_logic;
       reset     : in std_logic;
       count_out	: in	std_logic_vector (19 downto 0);
       pwm_reset : out std_logic
      );
  end component;
  
  --COUNTER
  component counter is
	port (	clk		: in	std_logic;
		     reset		: in	std_logic;
		     count_out	: out	std_logic_vector (19 downto 0)
	 );
  end component counter;
  
  signal clk, reset, pwm_reset : std_logic;
  signal count_out             : std_logic_vector (19 downto 0);
  
  begin
   clk           <=   '1' after 0 ns,
                      '0' after 10 ns when clk /= '0' else '1' after 10 ns;
   reset         <=   '1' after 0 ns, '0' after 400 ns;
   
   lb1: test_controller port map (clk, reset, count_out, pwm_reset);
   lb2: counter port map ( clk, reset, count_out);
     
end structural;   
   
   