library IEEE;
use IEEE.std_logic_1164.all;

-- De top-level entity
entity main is
	port (	
	  reset : in std_logic;
	  clk		: in	std_logic;

		sensor_l_in	: in	std_logic;
		sensor_m_in	: in	std_logic;
		sensor_r_in	: in	std_logic;

		servo_l_out	: out	std_logic;
		servo_r_out	: out	std_logic
	);
end entity main;

architecture structural of main is
  
  component inputbuffer is
	  port (	clk		: in	std_logic;

		       sensor_l_in	: in	std_logic;
		       sensor_m_in	: in	std_logic;
		       sensor_r_in	: in	std_logic;

		       sensor_l_out	: out	std_logic;
		       sensor_m_out	: out	std_logic;
		       sensor_r_out	: out	std_logic
	  );
  end component inputbuffer;
  
  component counter is
	   port (	clk		: in	std_logic;
		        reset		: in	std_logic;
		        count_out	: out	std_logic_vector (19 downto 0)
	   );
  end component counter;
  
  component controller is
	   port (	clk			: in	std_logic;
		        reset			: in	std_logic;

		        sensor_l		: in	std_logic;
		        sensor_m		: in	std_logic;
		        sensor_r		: in	std_logic;

		        count_in		: in	std_logic_vector (19 downto 0);
		        count_reset		: out	std_logic;

		        motor_l_reset		: out	std_logic;
		        motor_l_direction	: out	std_logic;

		        motor_r_reset		: out	std_logic;
		        motor_r_direction	: out	std_logic
	   );
  end component controller;
  
  component motorcontrol is
	   port (	clk		: in	std_logic;
		        reset		: in	std_logic;
		        direction	: in	std_logic;
		        count_in	: in	std_logic_vector (19 downto 0);

		        pwm		: out	std_logic
	   );
  end component motorcontrol;
  
  signal S_sensor_l_inputbuffer, S_sensor_m_inputbuffer, S_sensor_r_inputbuffer,
         S_count_reset, S_motor_l_reset, S_motor_l_direction,
         S_motor_r_reset, S_motor_r_direction: std_logic;
  signal S_count: std_logic_vector (19 downto 0);
  begin
    C_input_buffer: inputbuffer port map    ( clk, 
                                              sensor_l_in, sensor_m_in, sensor_r_in,
                                              S_sensor_l_inputbuffer, S_sensor_m_inputbuffer, S_sensor_r_inputbuffer );
    C_controller: controller port map       ( clk, reset, 
                                              S_sensor_l_inputbuffer, S_sensor_m_inputbuffer, S_sensor_r_inputbuffer,
                                              S_count, S_count_reset, S_motor_l_reset, S_motor_l_direction, 
                                              S_motor_r_reset, S_motor_r_direction );
    C_counter: counter port map             ( clk, S_count_reset, S_count 
                                            );
    C_motorcontrol_r: motorcontrol port map ( clk, S_motor_r_reset, S_motor_r_direction, S_count, servo_r_out 
                                            );
    C_motorcontrol_l: motorcontrol port map ( clk, S_motor_l_reset, S_motor_l_direction, S_count, servo_l_out 
                                            );
end architecture structural;