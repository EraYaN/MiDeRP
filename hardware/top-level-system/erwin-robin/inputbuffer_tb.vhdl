library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;

entity inputbuffer_tb is
end entity inputbuffer_tb;

architecture structural of inputbuffer_tb is
	
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
	
	signal clk			: 	std_logic;
	signal sensors, sensors_out : std_logic_vector(2 downto 0);	
begin

	clk		<=	'1' after 0 ns,
				'0' after 10 ns when clk /= '0' else '1' after 10 ns;
	sensors <= "000" after 23 ns,
	"010" after 234 ns,
	"001" after 435 ns,
	"101" after 567 ns,
	"111" after 574 ns,
	"001" after 613 ns,
	"100" after 654 ns,
	"110" after 684 ns,
	"101" after 693 ns,
	"111" after 704 ns,
	"000" after 754 ns;

l1:	inputbuffer port map (
					clk		=>	clk,
					sensor_l_in=>sensors(0),
					sensor_m_in=>sensors(1),
					sensor_r_in=>sensors(2),
					sensor_l_out=>sensors_out(0),
					sensor_m_out=>sensors_out(1),
					sensor_r_out=>sensors_out(2)
					);
					
end architecture structural;
