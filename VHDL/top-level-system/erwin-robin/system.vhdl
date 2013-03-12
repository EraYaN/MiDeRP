library IEEE;
use IEEE.std_logic_1164.all;

entity system is
	port (
	   clk		: in	std_logic; --kristal;
		reset		: in	std_logic; -- button;		
		
		sensor_l_in	: in	std_logic;
		sensor_m_in	: in	std_logic;
		sensor_r_in	: in	std_logic;
		pwm_motor_l		: out	std_logic;
		pwm_motor_r		: out	std_logic
	);
end entity system;

architecture structural of system is
    --components
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
	component motorcontrol is
		port (	clk		: in	std_logic;
			reset		: in	std_logic;
			direction	: in	std_logic;
			count_in	: in	std_logic_vector (19 downto 0);

			pwm		: out	std_logic
		);
	end component motorcontrol;
	component timebase is
		port (	clk		: in	std_logic;
			reset		: in	std_logic;

			count_out	: out	std_logic_vector (19 downto 0)
		);
	end component timebase;
     --signals 
    signal count : std_logic_vector (19 downto 0);
	signal sensors : std_logic_vector (2 downto 0); -- (left,middle,right)
    signal m_direction_l, m_direction_r, count_reset, m_reset_l, m_reset_r : std_logic;  
    begin
    IB: inputbuffer port map(
		clk => clk,
		sensor_l_in => sensor_l_in,
		sensor_r_in => sensor_r_in,
		sensor_m_in => sensor_m_in,
		sensor_l_out => sensors(0),
		sensor_r_out => sensors(1),
		sensor_m_out => sensors(2)
    );
    MCL: motorcontrol port map(
		clk => clk,
		reset => m_reset_l,
		direction => m_direction_l,
		count_in => count,
		pwm => pwm_motor_l
    );
    MCR: motorcontrol port map(
		clk => clk,
		reset => m_reset_r,
		direction => m_direction_r,
		count_in => count,
		pwm => pwm_motor_r
    );
    TB: timebase port map(
		clk => clk,
		reset => count_reset,
		count_out => count
    );
    C: controller port map(
		clk => clk,
		reset => reset,
		count_in => count,		

		sensor_l => sensors(0),
		sensor_m => sensors(1),
		sensor_r => sensors(2),
		
		count_reset => count_reset,

		motor_l_reset => m_reset_l,
		motor_l_direction => m_direction_l,

		motor_r_reset => m_reset_r,
		motor_r_direction => m_direction_r
    );    
        
        
end architecture structural;