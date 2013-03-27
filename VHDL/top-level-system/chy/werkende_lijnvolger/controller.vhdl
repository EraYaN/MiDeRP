library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.numeric_std.all;

-- De top-level entity
entity controller is
	port (	clk			: in	std_logic;
		reset			: in	std_logic;

		sensor_l		: in	std_logic;
		sensor_m		: in	std_logic;
		sensor_r		: in	std_logic;

		count_in		: in	std_logic_vector (19 downto 0);
		count_reset		: out	std_logic;

		motor_l_reset		: out	std_logic;
		motor_l_direction	: out	std_logic;

		motor_r_reset		: out	std_logic;
		motor_r_direction	: out	std_logic
	);
end entity controller;

architecture behavioural of controller is
  type test_controller_state is ( read_state, III_state, OOO_state, OOI_state, OIO_state, OII_state,
                                              IOO_state, IOI_state, IIO_state
                                 );
  signal state, new_state : test_controller_state;

begin
  process ( clk )
  begin 
    if ( rising_edge ( clk ) ) then 
      if ( reset = '1' ) then 
        state <= read_state;
      else 
        state <= new_state;
      end if;
    end if;
  end process;
  
  process ( state, count_in, sensor_l, sensor_m, sensor_r )
  begin 
    case state is 
    when read_state =>
      count_reset <= '1';
      if ((sensor_l = '1') AND (sensor_m = '1') AND (sensor_r = '1')) then
        new_state <= III_state;
      elsif ((sensor_l = '0') AND (sensor_m = '0') AND (sensor_r = '0')) then
        new_state <= OOO_state;
      elsif ((sensor_l = '0') AND (sensor_m = '0') AND (sensor_r = '1')) then
        new_state <= OOI_state;
      elsif ((sensor_l = '0') AND (sensor_m = '1') AND (sensor_r = '0')) then
        new_state <= OIO_state;
      elsif ((sensor_l = '0') AND (sensor_m = '1') AND (sensor_r = '1')) then
        new_state <= OII_state;
      elsif ((sensor_l = '1') AND (sensor_m = '0') AND (sensor_r = '0')) then
        new_state <= IOO_state;
      elsif ((sensor_l = '1') AND (sensor_m = '0') AND (sensor_r = '1')) then
        new_state <= IOI_state;
      elsif ((sensor_l = '1') AND (sensor_m = '1') AND (sensor_r = '0')) then
        new_state <= IIO_state;
      elsif ((sensor_l = '1') AND (sensor_m = '1') AND (sensor_r = '1')) then
        new_state <= III_state;
      end if;
      
    --BEIDE VOORUIT    
    when III_state =>
      count_reset <= '0';
    		motor_l_reset <= '0';
		  motor_l_direction <= '1';

		  motor_r_reset <= '0';
		  motor_r_direction <= '0';
      
      if ( unsigned( count_in ) > 999997 ) then
        new_state <= read_state;
      else 
        new_state <= III_state;
      end if;
    
    --BEIDE VOORUIT  
    when OOO_state =>
      count_reset <= '0';
      motor_l_reset <= '0';
    		motor_l_direction <= '1';

      motor_r_reset <= '0';
		  motor_r_direction <= '0';
		  
		  if ( unsigned( count_in ) > 999997 ) then
        new_state <= read_state;
      else 
        new_state <= OOO_state;
      end if;
		
	--LINKS VOORUIT, RECHTS STOP  
		when OOI_state =>
      count_reset <= '0';
    		motor_l_reset <= '0';
		  motor_l_direction <= '1';

		  motor_r_reset <= '1';
		  
      
      if ( unsigned( count_in ) > 999997 ) then
        new_state <= read_state;
      else 
        new_state <= OOI_state;
      end if;
      
   --BEIDE VOORUIT   
  		when OIO_state =>
      count_reset <= '0';
    		motor_l_reset <= '0';
		  motor_l_direction <= '1';

		  motor_r_reset <= '0';
		  motor_r_direction <= '0';
      
      if ( unsigned( count_in ) > 999997 ) then
        new_state <= read_state;
      else 
        new_state <= OIO_state;
      end if;
      
   --LINKS VOORUIT, RECHTS STOPPEN
  		when OII_state =>
      count_reset <= '0';
    		motor_l_reset <= '0';
		  motor_l_direction <= '1';

		  motor_r_reset <= '1';
      
      if ( unsigned( count_in ) > 999997 ) then
        new_state <= read_state;
      else 
        new_state <= OII_state;
      end if;
      
   --LINKS STOP, RECHTS VOORUIT
  		when IOO_state =>
      count_reset <= '0';
    		motor_l_reset <= '1';
		  

		  motor_r_reset <= '0';
		  motor_r_direction <= '0';
      
      if ( unsigned( count_in ) > 999997 ) then
        new_state <= read_state;
      else 
        new_state <= IOO_state;
      end if;
      
   --BEIDE VOORUIT
  		when IOI_state =>
      count_reset <= '0';
    		motor_l_reset <= '0';
		  motor_l_direction <= '1';

		  motor_r_reset <= '0';
		  motor_r_direction <= '0';
      
      if ( unsigned( count_in ) > 999997 ) then
        new_state <= read_state;
      else 
        new_state <= IOI_state;
      end if;
      
    --LINKS STOP, RECHTS VOORUIT
  		when IIO_state =>
      count_reset <= '0';
    		motor_l_reset <= '1';

		  motor_r_reset <= '0';
		  motor_r_direction <= '0';
      
      if ( unsigned( count_in ) > 999997 ) then
        new_state <= read_state;
      else 
        new_state <= IIO_state;
      end if;
      
    end case;
  end process;
end architecture behavioural;