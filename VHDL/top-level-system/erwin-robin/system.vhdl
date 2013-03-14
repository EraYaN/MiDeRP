library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;
library std;
use std.textio.all;

entity system is
	port (
		clk		: in	std_logic; --kristal;
		reset		: in	std_logic; -- button;		
		
		sensor		: in	std_logic_vector (2 downto 0); -- left = 0 middle = 1 right = 2
		servo		: out	std_logic_vector (1 downto 0); -- left = 0 right = 1
		
		--uart
		rx: in std_logic;
		tx: in std_logic	
		
	);
end entity system;

architecture structural of system is
    --components
    component controller is
		port (	clk			: in	std_logic;
			reset			: in	std_logic;

			sensor		: in	std_logic_vector(2 downto 0);	

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
			sensor_in	: in	std_logic_vector(2 downto 0);
			sensor_out	: out	std_logic_vector(2 downto 0)
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
	
	component uart is
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
	end component uart;
	
     --signals 
    signal count : unsigned (19 downto 0);
	signal bufferedsensor : std_logic_vector (2 downto 0); -- (left,middle,right)
    signal count_reset, m_reset_l, m_reset_r : std_logic := '0';  
	signal side_l : side := left;
	signal side_r : side := right;
	signal m_speed_r, m_speed_l : signed (7 downto 0) := "00000000";
    begin
    IB: inputbuffer port map(
		clk => clk,
		sensor_in => sensor,
		sensor_out => bufferedsensor
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

		sensor => bufferedsensor,	
		
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