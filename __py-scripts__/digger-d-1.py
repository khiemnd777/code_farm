from builtins import *
from __builtins__.digger import *

# The function will be called when running
async def __main():
    i = 0
    while i < 5:
        move_forward()
        i += 1