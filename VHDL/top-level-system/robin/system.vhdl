library IEEE;
use IEEE.std_logic_1164.all;

entity system is
	port (
		sensors : in std_logic_vector (2 downto 0);
		clk		: in std_logic;
		reset 	: in std_logic;
		
		pwm 	: out std_logic_vector (1 downto 0)
	);
end entity system;

architecture structural of system is
	component controller is
		port (
			clk			: in	std_logic;
			reset		: in	std_logic;

			sensor_l	: in	std_logic;
			sensor_m	: in	std_logic;
			sensor_r	: in	std_logic;

			count_in	: in	std_logic_vector (19 downto 0);
			count_reset	: out	std_logic;

			motor_l_reset		: out	std_logic;
			motor_l_direction	: out	std_logic;

			motor_r_reset		: out	std_logic;
			motor_r_direction	: out	std_logic
		);
	end component controller;
	
	component inputbuffer is
		port (	
			clk		: in	std_logic;

			sensor_l_in	: in	std_logic;
			sensor_m_in	: in	std_logic;
			sensor_r_in	: in	std_logic;

			sensor_l_out	: out	std_logic;
			sensor_m_out	: out	std_logic;
			sensor_r_out	: out	std_logic
		);
	end component inputbuffer;
	
	component motorcontrol is
		port (	
			clk			: in	std_logic;
			reset		: in	std_logic;
			direction	: in	std_logic;
			count_in	: in	std_logic_vector (19 downto 0);

			pwm		: out	std_logic
		);
	end component motorcontrol;
	
	component timebase is
		port (	
			clk			: in	std_logic;
			reset		: in	std_logic;

			count_out	: out	std_logic_vector (19 downto 0)
		);
	end component timebase;
	
	signal count_reset : std_logic;
	signal count : std_logic_vector (19 downto 0);
	signal sensor_sig : std_logic_vector (2 downto 0);
	signal motor_dir, motor_reset : std_logic_vector (1 downto 0);

begin
	l1: controller port map
					(
						clk 				=>	clk,
						reset				=>	reset,
						sensor_l			=>	sensor_sig(0),
						sensor_m			=>	sensor_sig(1),
						sensor_r			=>	sensor_sig(2),
						count_in			=>	count,
						count_reset			=>	count_reset,
						motor_l_reset		=>	motor_reset(0),
						motor_l_direction	=>	motor_dir(0),
						motor_r_reset		=>	motor_reset(1),
						motor_r_direction	=>	motor_dir(1)
					);
					
	l2: inputbuffer port map
					(
						clk					=>	clk,
						sensor_l_in			=>	sensors(0),
						sensor_m_in			=>	sensors(1),
						sensor_r_in			=>	sensors(2),
						sensor_l_out		=>	sensor_sig(0),
						sensor_m_out		=>	sensor_sig(1),
						sensor_r_out		=>	sensor_sig(2)
					);
					
	l3: motorcontrol port map --left
					(
						clk					=>	clk,
						reset				=>	reset,
						direction			=>	motor_dir(0),
						count_in			=>	count,
						pwm					=>	pwm(0)
					);
					
	l4: motorcontrol port map --right
					(
						clk					=>	clk,
						reset				=>	reset,
						direction			=>	motor_dir(1),
						count_in			=>	count,
						pwm					=>	pwm(1)
					);

	l5: timebase port map
					(
						clk					=>	clk,
						reset				=>	reset,
						count_out			=>	count
					);
						
	
end architecture structural;
				