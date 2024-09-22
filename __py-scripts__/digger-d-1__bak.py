from builtins import *
from __builtins__.digger import *

# Moving and rotating func
async def move_and_rotate (counterclockwise: bool = False):
    i = 0
    while i < 4:
        await move_forward()
        await move_forward()
        await dig()
    
        if counterclockwise:
            await rotate_counterclockwise()
        else:
            await rotate_clockwise()

        i += 1
     
# 1st moving func
async def move_1():
    while True:
        await move_to(2, 3)
        await dig()
        await move_to(0, 0)
        
        i = 0
        while i < 5:
            await rotate_clockwise()
            i += 1
        await move_to(2, 1)
        await dig()
        await move_to(0, 0)
        
        i = 0
        while i < 3:
            await rotate_counterclockwise()
            i += 1
           
# 2nd moving func
async def move_2():
    await move_to(0, 0)
    await move_and_rotate()
    await move_and_rotate(True)
    await move_to(0, 0)
    i = 0
    while i < 3:
        await rotate_counterclockwise()
        i += 1
    
# 3rd moving func
async def move_3():
    await move_to(49, 49)
    await move_to(0, 0)
    i = 0
    while i < 3:
        await rotate_counterclockwise()
        i += 1
        
# The function will be called when running
async def __main():
    await move_3()
    await move_2()
    await move_1()
