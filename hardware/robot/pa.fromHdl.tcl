
# PlanAhead Launch Script for Pre-Synthesis Floorplanning, created by Project Navigator

hdi::project new -name robot -dir "H:/GitHub/MiDeRP/hardware/robot/patmp"
hdi::project setArch -name robot -arch spartan3e
hdi::design setOptions -project robot -top system  
hdi::param set -name project.paUcfFile -svalue "H:/GitHub/MiDeRP/hardware/robot/code/system.ucf"
hdi::floorplan new -name floorplan_1 -part xc3s250ecp132-4 -project robot
hdi::port import -project robot \
    -vhdl {code/uart_tx.vhdl work} \
    -vhdl {code/uart_rx.vhdl work} \
    -vhdl {code/buf_reg.vhdl work} \
    -vhdl {code/baud_gen.vhdl work} \
    -vhdl {code/3-bitregister.vhdl work} \
    -vhdl {code/uart.vhdl work} \
    -vhdl {code/timebase.vhdl work} \
    -vhdl {code/ssegdecoder.vhdl work} \
    -vhdl {code/motorcontrol.vhdl work} \
    -vhdl {code/inputbuffer.vhdl work} \
    -vhdl {code/controller.vhdl work} \
    -vhdl {code/system.vhdl work}
hdi::port export -project robot -file robot_pa_ports.v -format verilog
hdi::pconst import -project robot -floorplan floorplan_1 -file "H:/GitHub/MiDeRP/hardware/robot/code/system.ucf"
