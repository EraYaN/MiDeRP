library IEEE;
use IEEE.std_logic_1164.all;

-- De top-level entity
entity main is
	port (	
	  reset : in std_logic;
	  clk		: in	std_logic;

		switch : in std_logic;
		
		pwm	: out	std_logic
	);
end entity main;

architecture structural of main is

  
  component timebase is
	   port (	clk		: in	std_logic;
		        reset		: in	std_logic;
		        count_out	: out	std_logic_vector (19 downto 0)
	   );
  end component timebase;
  
  component test_controller is
	   port (	clk			: in	std_logic;
		        reset			: in	std_logic;

		        count_in		: in	std_logic_vector (19 downto 0);
		        count_reset		: out	std_logic
	   );
  end component test_controller;
  
  component motorcontrol is
	   port (	clk		: in	std_logic;
		        reset		: in	std_logic;
		        direction	: in	std_logic;
		        count_in	: in	std_logic_vector (19 downto 0);

		        pwm		: out	std_logic
	   );
  end component motorcontrol;
  
  signal S_count_reset: std_logic;
  signal S_count: std_logic_vector (19 downto 0);
  begin

    C_controller: test_controller port map       ( clk, reset, S_count, S_count_reset);
    C_timebase: timebase port map                ( clk, reset, S_count);
    C_motorcontrol_r: motorcontrol port map      ( clk, reset, switch, S_count, pwm);
end architecture structural;