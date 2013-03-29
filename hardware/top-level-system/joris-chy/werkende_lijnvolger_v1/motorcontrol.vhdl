library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;

-- De top-level entity
entity motorcontrol is
	port (	clk		: in	std_logic;
		     reset		: in	std_logic;
		     direction	: in	std_logic; -- 0 == links en 1 == rechts
		  	  count_in	: in	std_logic_vector (19 downto 0);

		     pwm		: out	std_logic
	);
end entity motorcontrol;

architecture behavioural of motorcontrol is
  
  type motor_controller_state is ( pwm_laag, pwm_hoog, reset_state);
    
  signal state, new_state: motor_controller_state;
  
begin
process ( clk )
begin
  if ( rising_edge (clk)) then
    if ( reset = '1' ) then
      state <= reset_state;
    else
      state <= new_state;
    if ( (unsigned(count_in) < 100000) and ( direction = '1')) or ((unsigned(count_in) < 50000) and ( direction = '0')) then
        new_state <= pwm_hoog;
    else
        new_state <= pwm_laag;
    end if;
    end if;
    end if;
end process;

process ( state )
  begin
    case state is 
    when pwm_laag => 
      pwm <= '0';
      
    when pwm_hoog => 
      pwm <= '1';
      
    when reset_state =>
      pwm <= '0';
  end case;
  end process;
end behavioural;