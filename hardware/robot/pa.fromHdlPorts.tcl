
# PlanAhead Launch Script for Pre-Synthesis Floorplanning, created by Project Navigator

hdi::project new -name robot -dir "H:/GitHub/MiDeRP/hardware/robot/patmp"
hdi::project setArch -name robot -arch spartan3e
hdi::design setOptions -project robot -top netlist_1_EMPTY
hdi::param set -name project.paUcfFile -svalue "H:/GitHub/MiDeRP/hardware/robot/code/system.ucf"
hdi::floorplan new -name floorplan_1 -part xc3s250ecp132-4 -project robot
hdi::port import -project robot -verilog {robot_pa_ports.v work}
hdi::pconst import -project robot -floorplan floorplan_1 -file "H:/GitHub/MiDeRP/hardware/robot/code/system.ucf"
