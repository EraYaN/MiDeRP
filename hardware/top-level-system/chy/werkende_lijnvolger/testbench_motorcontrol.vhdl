library IEEE;
use IEEE.std_logic_1164.all;

entity testbench_motorcontrol is
end entity testbench_motorcontrol;

architecture structural of testbench_motorcontrol is
  component test_controller is
port ( clk       : in std_logic;
       reset     : in std_logic;
       count_out	: in	std_logic_vector (19 downto 0);
       pwm_reset : out std_logic
      );
      
end component test_controller;

   component motorcontrol is
           port( clk		: in	std_logic;
		             reset		: in	std_logic;
		             direction	: in	std_logic; -- 0 == links en 1 == rechts
		  	          count_in	: in	std_logic_vector (19 downto 0);

		             pwm		: out	std_logic );
   end component; 
   
component counter is
	port (	clk		: in	std_logic;
		     reset		: in	std_logic;
		     count_out	: out	std_logic_vector (19 downto 0)
	 );
end component counter;
     
signal clk, reset, direction, pwm, pwm_reset : std_logic;
signal count_in : std_logic_vector (19 downto 0);
    
begin
   clk           <=   '1' after 0 ns,
                      '0' after 10 ns when clk /= '0' else '1' after 10 ns;
   reset         <=   '1' after 0 ns, '0' after 125 ns;
   
   direction   <=   '1' after 0 ns, '0' after 15000000 ns;
      lb1: test_controller port map (clk, reset, count_in, pwm_reset);
      lb2: motorcontrol port map (clk, reset, direction, count_in, pwm);
      lb3: counter port map ( clk, pwm_reset, count_in); 
end structural;