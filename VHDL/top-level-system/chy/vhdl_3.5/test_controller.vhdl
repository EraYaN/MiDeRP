library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;

entity test_controller is
port ( clk       : in std_logic;
       reset     : in std_logic;
       count_out	: in	std_logic_vector (19 downto 0);
       pwm_reset : out std_logic
      );
      
end entity test_controller;

architecture behavioural of test_controller is
  type test_controller_state is ( reset_state,
                                  wait_state
                                 );
  signal state, new_state : test_controller_state;

begin
  process ( clk )
  begin 
    if ( rising_edge ( clk ) ) then 
      if ( reset = '1' ) then 
        state <= reset_state;
      else 
        state <= new_state;
      end if;
    end if;
  end process;
  
  process ( state, count_out )
  begin 
    case state is 
    --WAIT_STATE
    when wait_state =>
      --output_signal
      pwm_reset <= '0';
      --new_state
      if ( unsigned( count_out ) > 999999 ) then
        new_state <= reset_state;
      else 
        new_state <= wait_state;
      end if;
    
    --RESET_STATE
    when reset_state =>
      pwm_reset <= '1';
      --newstate
      new_state <= wait_state;
    end case;
  end process;
end architecture behavioural;
 
  
 