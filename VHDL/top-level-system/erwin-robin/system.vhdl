library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;
library std;
use std.textio.all;

entity system is
	port (
	   clk		: in	std_logic; --kristal;
		reset		: in	std_logic; -- button;		
		
		sensor		: in	std_logic_vector (2 downto 0);
		servo		: out	std_logic_vector (1 downto 0)
		---debug_m_speed_l : out signed (7 downto 0);
		---debug_m_speed_r : out signed (7 downto 0);
		---debug_count : out unsigned (19 downto 0)
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

			count_in		: in	unsigned (19 downto 0);
			count_reset		: out	std_logic;

			motor_l_reset		: out	std_logic;
			motor_l_speed		: out	signed (7 downto 0);

			motor_r_reset		: out	std_logic;
			motor_r_speed		: out	signed (7 downto 0)
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
			speed		: in	signed (7 downto 0); -- van 100 tot -100
			count_in	: in	unsigned (19 downto 0);
			motor		: in	side; --0=left 1=right

			pwm		: out	std_logic
		);
	end component motorcontrol;
	component counter is
		port (	clk		: in	std_logic;
			reset		: in	std_logic;

			count_out	: out	unsigned (19 downto 0)
		);
	end component counter;
     --signals 
    signal count : unsigned (19 downto 0);
	signal bufferedsensors : std_logic_vector (2 downto 0); -- (left,middle,right)
    signal count_reset, m_reset_l, m_reset_r : std_logic := '0';  
	signal side_l : side := left;
	signal side_r : side := right;
	signal m_speed_r, m_speed_l : signed (7 downto 0) := "00000000";
    begin
    IB: inputbuffer port map(
		clk => clk,
		sensor_l_in => sensor(0),
		sensor_r_in => sensor(1),
		sensor_m_in => sensor(2),
		sensor_l_out => bufferedsensors(0),
		sensor_r_out => bufferedsensors(1),
		sensor_m_out => bufferedsensors(2)
    );
    MCL: motorcontrol port map(
		clk => clk,
		reset => m_reset_l,
		speed => m_speed_l,
		count_in => count,
		pwm => servo(0),
		motor => side_l
    );
    MCR: motorcontrol port map(
		clk => clk,
		reset => m_reset_r,
		speed => m_speed_r,
		count_in => count,
		pwm => servo(1),
		motor => side_r
    );
    TB: counter port map(
		clk => clk,
		reset => count_reset,
		count_out => count
    );
    C: controller port map(
		clk => clk,
		reset => reset,
		count_in => count,		

		sensor_l => bufferedsensors(0),
		sensor_m => bufferedsensors(1),
		sensor_r => bufferedsensors(2),
		
		count_reset => count_reset,

		motor_l_reset => m_reset_l,
		motor_l_speed => m_speed_l,

		motor_r_reset => m_reset_r,
		motor_r_speed => m_speed_r
    );    
       ---debug_m_speed_l<=m_speed_l; 
       ---debug_m_speed_r<=m_speed_r;
	---debug_count<=count;	   
end architecture structural;