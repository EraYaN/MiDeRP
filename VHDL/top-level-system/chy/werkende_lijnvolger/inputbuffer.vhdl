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
  
component d_ff is
	port (d, clk : in std_logic; 
	      q : out std_logic);
end component d_ff;


    signal S_register_l, S_register_m, S_register_r: std_logic;
    begin
      C_dff_0: d_ff port map ( sensor_l_in, clk, S_register_l );
      C_dff_1: d_ff port map ( sensor_m_in, clk, S_register_m );
      C_dff_2: d_ff port map ( sensor_r_in, clk, S_register_r );
        
      C_dff_3: d_ff port map ( S_register_l, clk, sensor_l_out );
      C_dff_4: d_ff port map ( S_register_m, clk, sensor_m_out );
      C_dff_5: d_ff port map ( S_register_r, clk, sensor_r_out );
end structural;

library IEEE;
use IEEE.std_logic_1164.all;

entity d_ff is
	port (d, clk : in std_logic; q : out std_logic);
end d_ff;

architecture gedrag_dff of d_ff is
begin
	ff_behavior : process is
	begin
		wait until (clk'event and clk='1');
			q <= d;
	end process;
end gedrag_dff;
