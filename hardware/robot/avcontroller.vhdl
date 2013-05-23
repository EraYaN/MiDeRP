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
signal count : integer;
signal count2 : integer;
signal state : av_state;
signal piezo:std_logic;
begin
process(clk) is
variable bin : std_logic_vector(3 downto 0):=x"F";
variable next_state : av_state;
variable next_count,next_count2 : integer;
variable next_piezo : std_logic;

begin
	if rising_edge(clk) then
		next_state:= state;
		next_piezo := piezo;
		next_count := count;
		next_count2 := count2;
		if reset = '1' then
			next_piezo := '0';
			led_out <= "00";
			next_count := 0;	
			next_count2 := 0;			
		elsif state = silent then		
			if minedetected = '1' then
				led_out <= "11";
				next_count2 := 1200;
				next_state := siren;
			else
				led_out <= "00";
				next_piezo := '0';
			end if;
		elsif state = siren then	
			led_out <= "11"; 
			if count = 0 then
				next_piezo := not piezo;
				next_count := 20000;
				next_count2:=count2-1;							
			else
				next_count:=count-1;				
			end if;
			if count2 = 0 then
				next_state:=silent;
			end if;
		end if;	
		piezo <= next_piezo;
		state<=next_state;
		count<=next_count;
		count2<=next_count2;
	end if;
end process;
piezo_out<=piezo;
end Behavioral;