library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;

entity system_tb is
end entity system_tb;

architecture structural of system_tb is
	
	component system is
		port (
		   clk		: in	std_logic; --kristal;
			reset		: in	std_logic; -- button;		
			
			sensor_l_in	: in	std_logic;
			sensor_m_in	: in	std_logic;
			sensor_r_in	: in	std_logic;
			pwm_motor_l		: out	std_logic;
			pwm_motor_r		: out	std_logic;
			---debug_m_speed_l : out signed (7 downto 0);
			---debug_m_speed_r : out signed (7 downto 0);
			---debug_count : out unsigned (19 downto 0)
		);
	end component system;
	
	signal clk, pwm_l, pwm_r	: 	std_logic;
	signal reset		:	std_logic := '1' ;
	signal sensors	:	std_logic_vector (2 downto 0) := "000";
	--signal msl, msr : signed (7 downto 0);
	--signal count : unsigned (19 downto 0);
begin

	clk		<=	'1' after 0 ns,
				'0' after 10 ns when clk /= '0' else '1' after 10 ns;
	reset	<=	'0' after 40 ns,
				'1' after 460 us,
				'0' after 461 us;
	sensors <= "001" after 40 ms,
				"010" after 80 ms,
				"011" after 120 ms,
				"100" after 160 ms,
				"101" after 200 ms,
				"110" after 240 ms,
				"111" after 380 ms,
				"000" after 420 ms;
sys:	system port map (
	clk		=>	clk,
	reset	=>	reset,
	sensor_l_in	=>	sensors(0),
	sensor_m_in	=>	sensors(1),
	sensor_r_in	=>	sensors(2),
	pwm_motor_l => pwm_l,
	pwm_motor_r => pwm_r
	--debug_m_speed_l => msl,
	--debug_m_speed_r => msr,
	--debug_count => count
);
					
end architecture structural;