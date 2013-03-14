library IEEE;
use IEEE.std_logic_1164.all;

entity inputbuffer is
	port (	clk		: in	std_logic;

		sensor_in	: in	std_logic_vector(2 downto 0);		

		sensor_out	: out	std_logic_vector(2 downto 0)
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
		input=>sensor_in,		
		output=>tussen,
		reset=>gnd
	);
	d2:threebitregister port map(
		clk => clk,
		output=>sensor_out,		
		input=>tussen,
		reset=>gnd
	);
	
end architecture struct;
