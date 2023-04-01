from builtins import *
from __builtins__.digger import *

async def move_and_rotate (counterclockwise: bool = False):
  i = 0
  while i < 4:
    await move_forward()    
    await dig()
    
    if counterclockwise:
      await rotate_counterclockwise()
    else:
      await rotate_clockwise()

    i += 1
    
async def __main():
  await move_to(0, 0)
  # await move_and_rotate()
  # await move_and_rotate(True)
