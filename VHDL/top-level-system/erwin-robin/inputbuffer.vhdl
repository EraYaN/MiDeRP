library IEEE;
use IEEE.std_logic_1164.all;

entity inputbuffer is
	port (	clk		: in	std_logic;

		sensor_l_in	: in	std_logic;
		sensor_m_in	: in	std_logic;
		sensor_r_in	: in	std_logic;

		sensor_l_out	: out	std_logic;
		sensor_m_out	: out	std_logic;
		sensor_r_out	: out	std_logic
	);
end entity inputbuffer;

architecture struct of inputbuffer is
	component threebitregister is
		port (	clk		: in	std_logic;
		reset : in std_logic;
		input : in std_logic_vector (2 downto 0);

		output : out std_logic_vector (2 downto 0)
	);
	end component threebitregister;
	signal tussen : std_logic_vector (2 downto 0);
	signal gnd : std_logic :='0';
begin
	d1:threebitregister port map(
		clk => clk,
		input(0)=>sensor_l_in,
		input(1)=>sensor_m_in,
		input(2)=>sensor_r_in,
		output=>tussen,reset=>gnd
	);
	d2:threebitregister port map(
		clk => clk,
		output(0)=>sensor_l_out,
		output(1)=>sensor_m_out,
		output(2)=>sensor_r_out,
		input=>tussen,reset=>gnd
	);
	
end architecture struct;
