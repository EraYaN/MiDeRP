library IEEE;
use IEEE.std_logic_1164.all;

entity testbench_main is
end entity testbench_main;

architecture structural of testbench_main is
  component main is
	port (	
	  reset : in std_logic;
	  clk		: in	std_logic;

		sensor_l_in	: in	std_logic;
		sensor_m_in	: in	std_logic;
		sensor_r_in	: in	std_logic;

		servo_l_out	: out	std_logic;
		servo_r_out	: out	std_logic
	);
end component main;
        
        
signal reset, clk, sensor_l_in, sensor_m_in, sensor_r_in, servo_l_out, servo_r_out : std_logic;
    
begin
   clk           <=   '1' after 0 ns,
                      '0' after 10 ns when clk /= '0' else '1' after 10 ns;
   reset         <=   '1' after 0 ns, '0' after 30 ns;
   sensor_l_in   <=   '0'after 0 ns, '1' after 20 ns, '1' after 25000000 ns;
   sensor_m_in   <=   '0'after 0 ns, '1' after 20 ns, '0' after 25000000 ns;
   sensor_r_in   <=   '0'after 0 ns, '1' after 20 ns, '0' after 25000000 ns;
   lb1: main port map (reset, clk, sensor_l_in, sensor_m_in, sensor_r_in, servo_l_out, servo_r_out);
end structural;  