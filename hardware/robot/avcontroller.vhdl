library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
entity avcontroller is
	port(
		clk : in std_logic;
		reset : in std_logic;
		minedetected : in std_logic;
		piezo_out: out std_logic;
		led_out: out std_logic_vector(1 downto 0)
	);
end avcontroller;

architecture Behavioral of avcontroller is
type av_state is (silent,siren);
signal counter : integer;
signal state : av_state;
signal piezo:std_logic;
begin
process(clk) is
variable bin : std_logic_vector(3 downto 0):=x"F";
variable next_state : av_state;
variable next_counter : integer;
variable next_piezo : std_logic;

begin
	if rising_edge(clk) then
		next_state:= state;
		next_piezo := piezo;
		next_counter := 0;
		if reset = '1' then
			next_piezo := '0';
			led_out <= "00";
			next_counter := 0;			
		elsif state = silent then		
			if minedetected = '1' then
				led_out <= "11";
				next_state := siren;
			else
				led_out <= "00";
				next_piezo := '0';
			end if;
		elsif state = siren then	
			led_out <= "11"; 
			if counter > 85000 then
				next_piezo := not piezo;
				next_counter := 0;
			else
				next_counter:=counter+1;				
			end if;
		end if;	
		piezo <= next_piezo;
		state<=next_state;
		counter<=next_counter;
	end if;
end process;
piezo_out<=piezo;
end Behavioral;