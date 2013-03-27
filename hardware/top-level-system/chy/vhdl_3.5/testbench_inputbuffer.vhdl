library IEEE;
use IEEE.std_logic_1164.all;

entity testbench_inputbuffer is
end entity testbench_inputbuffer;

architecture structural of testbench_inputbuffer is
    component inputbuffer is
           port( clk   : in std_logic;
		             sensor_l_in	: in	std_logic;
		             sensor_m_in	: in	std_logic;
		             sensor_r_in	: in	std_logic;

		             sensor_l_out	: out	std_logic;
		             sensor_m_out	: out	std_logic;
		             sensor_r_out	: out	std_logic);
   end component; 
        
        
signal clk, sensor_l_in, sensor_m_in, sensor_r_in, sensor_l_out, sensor_m_out, sensor_r_out : std_logic;
    
begin
   clk           <=   '1' after 0 ns,
                      '0' after 50 ns when clk /= '0' else '1' after 50 ns;
   sensor_l_in   <=   '0'after 0 ns, '1' after 75 ns, '0' after 125 ns, '1' after 175 ns, '0' after 225 ns;
   sensor_m_in   <=   '0'after 0 ns, '1' after 75 ns, '0' after 125 ns, '1' after 175 ns, '0' after 225 ns;
   sensor_r_in   <=   '0'after 0 ns, '1' after 75 ns, '0' after 125 ns, '1' after 175 ns, '0' after 225 ns;
   lb1: inputbuffer port map (clk, sensor_l_in, sensor_m_in, sensor_r_in, sensor_l_out, sensor_m_out, sensor_r_out);
end structural;  
