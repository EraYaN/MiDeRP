library IEEE;
use IEEE.std_logic_1164.all;


-- De top-level entity
entity inputbuffer is
	port (	clk		: in	std_logic;

		sensor_l_in	: in	std_logic;
		sensor_m_in	: in	std_logic;
		sensor_r_in	: in	std_logic;

		sensor_l_out	: out	std_logic;
		sensor_m_out	: out	std_logic;
		sensor_r_out	: out	std_logic
	);
end inputbuffer;

architecture structural of inputbuffer is
  
component register_3bit is
	  port ( 		
	  clk		: in	std_logic;
	  
	  register_l_in	: in	std_logic;
		register_m_in	: in	std_logic;
		register_r_in	: in	std_logic;

		register_l_out	: out	std_logic;
		register_m_out	: out	std_logic;
		register_r_out	: out	std_logic
	  );
end component register_3bit; 

    signal S_register_l, S_register_m, S_register_r: std_logic;
    begin
      C_register_3bit_1: register_3bit port map ( clk, sensor_l_in, sensor_m_in, sensor_r_in, S_register_l, S_register_m, S_register_r);
      C_register_3bit_2: register_3bit port map ( clk, S_register_l, S_register_m, S_register_r, sensor_l_out, sensor_m_out, sensor_r_out);

end structural;

library IEEE;
use IEEE.std_logic_1164.all;

entity register_3bit is
	port (	clk		: in	std_logic;

		register_l_in	: in	std_logic;
		register_m_in	: in	std_logic;
		register_r_in	: in	std_logic;

		register_l_out	: out	std_logic;
		register_m_out	: out	std_logic;
		register_r_out	: out	std_logic);
end register_3bit;

architecture behavioural of register_3bit is
begin
process (clk)
begin
  if ( rising_edge (clk)) then
    register_l_out <= register_l_in;
    register_m_out <= register_m_in;
    register_r_out <= register_r_in;
  else
  end if;
end process;
end behavioural;