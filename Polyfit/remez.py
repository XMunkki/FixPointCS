#
# FixPointCS
#
# Copyright(c) 2018 Jere Sanisalo, Petri Kero
# 
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
# 
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
# 
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.
#

# Implementation of Remez algorithm for optimal polynomial fitting for usage in
# function approximations.

# References:
# - https://github.com/samhocevar/lolremez
# - https://www.cs.ox.ac.uk/files/1948/NA-08-20.pdf
# - https://news.ycombinator.com/item?id=10115336
# - http://sollya.gforge.inria.fr/

import math
import random
import mpmath as mp
import numpy.polynomial as P
import numpy.polynomial.chebyshev as C

# set precision
#dps = 30
dps = 120
mp.mp.dps = dps
mp.mp.pretty = True

def vzeros(size):
	return mp.matrix([0.0] * size)

def vones(size):
	return mp.matrix([1.0] * size)

def nth(size):
	return mp.matrix([0.0] * size + [1.0])

class Remez:
	def __init__(self, func, weightFunc, domain, order):
		#print('Remez()')
		self.func = func
		self.weightFunc = weightFunc
		self.domain = domain
		self.order = order
		self.limit = mp.mpf(f'1e-{dps}')
		self.errorThreshold = mp.mpf(f'1e-{dps}')
		self.initRemez()

	def initRemez(self):
		#print('Remez.init()')

		# constants for domain
		(xMin, xMax) = self.domain
		self.k1 = (xMax + xMin) * 0.5
		self.k2 = (xMax - xMin) * 0.5

		# initial estimates for function roots (where error == 0.0)
		size = self.order + 1
		roots = vzeros(size)
		fxn = vzeros(size)
		# \todo [petri] use linspace
		for i in range(size):
			roots[i] = (2.0 * i - self.order) / size
			fxn[i] = self.evalFunc(roots[i])

		# build matrix of Chebyshev evaluations
		system = mp.zeros(size)
		for order in range(size):
			for i in range(size):
				system[i,order] = mp.chebyt(order, roots[i])

		# solve system
		solved = system ** -1

		# compute Chebyshev weights of new approximation
		weights = vzeros(size)
		for n in range(size):
			weights[n] = mp.fdot(solved[n,:], fxn)
		#print(f'  weights: {weights.T}')

		# store roots & weights
		self.roots = roots
		self.weights = weights
		self.maxError = 1000.0

		# estimate error
		#est = [self.evalEstimate(x) for x in roots]
		#print('  est:', est)
		#print('  fxn:', fxn.T)

	def step(self):
		(control, maxErr) = self.findExtrema(self.roots)
		#print(f'  maxErr: {maxErr} ({-math.log(maxErr, 2.0)} bits)')
		# for x in control:
		# 	print(f'  ctrl: {x}')
		self.weights = self.remezStep(control)

		# update error
		errDiff = mp.fabs(maxErr - self.maxError)
		self.maxError = maxErr

		if errDiff < self.errorThreshold:
			return True

		self.roots = self.findRoots(control)
		return False

		# for (ax, bx, rx, rerr) in scannedRoots:
		# 	print(f'  scanned: {rx} in [{ax, bx}]')

		# scannedRoots = self.scanRoots(1000)
		# for ndx in range(len(self.roots)):
		# 	x = self.roots[ndx]
		# 	(ax, bx, rx, rerr) = scannedRoots[ndx]
		# 	print(f'  root: {x} [{ax}, {bx}] {rx}')
		# 	if x < ax or x > bx:
		# 		print('  ROOT FIND MISMATCH')

	def findRoot(self, ax, bx, aerr, berr):
		cx = (ax + bx) * 0.5
		cerr = self.evalEstimate(cx) - self.evalFunc(cx)

		if cerr == 0.0 or (bx - ax) <= self.limit:
			return (cx, cerr)
		else:
			if (aerr < 0.0 and cerr < 0.0) or (aerr > 0.0 and cerr > 0.0):
				(ax, aerr) = (cx, cerr)
			else:
				(bx, berr) = (cx, cerr)
			return self.findRoot(ax, bx, aerr, berr)

	def findRoots(self, control):
		roots = vzeros(self.order + 1)
		for n in range(self.order + 1):
			ax = control[n]
			bx = control[n+1]
			aerr = self.evalEstimate(ax) - self.evalFunc(ax)
			berr = self.evalEstimate(bx) - self.evalFunc(bx)
			(rx, rerr) = self.findRoot(ax, bx, aerr, berr)
			assert(rx >= ax and rx <= bx) # root must be inside search range
			roots[n] = rx
			#print(f'  root[{n}]: {rx} <{rerr}> || {ax} {bx}')
		return roots

	def remezStep(self, control):
		# eval at control points
		fxn = mp.matrix([self.evalFunc(c) for c in control])

		# create linear system with chebyshev polynomials
		size = self.order + 2
		system = mp.zeros(size)
		for n in range(self.order + 1):
			for i in range(self.order + 2):
				system[i,n] = mp.chebyt(n, control[i])

		# last column is oscillating error
		for i in range(size):
			sign = -1 if ((i & 1) == 0) else +1
			scale = 0.0 if i in [0, size-1] else 1.0
			system[i,size-1] = sign * scale * mp.fabs(self.evalWeight(control[i]))
		#print(system)

		# solve the system
		solved = system ** -1
		#print(solved)

		# compute polynomial estimate (as Chebyshev weights)
		weights = vzeros(size-1)
		for n in range(size-1):
			weights[n] = mp.fdot(solved[n,:], fxn)
		#print(f'  weights: {weights}')

		# estimate error
		# self.weights = weights
		# est = [self.evalEstimate(x) for x in control]
		# print('  est:', est)
		# print('  fxn:', fxn.T)

		return weights

	def findExtremum(self, ax, bx, cx, aerr, berr, cerr):
		# \todo [petri] implement golden ratio search?

		dx = (ax + bx) * 0.5
		derr = self.evalError(dx)

		# update coords based on error
		if derr < cerr:
			if dx > cx:
				(bx, berr) = (dx, derr)
			else:
				(ax, aerr) = (dx, derr)
		else:
			if dx > cx:
				(ax, aerr) = (cx, cerr)
			else:
				(bx, berr) = (cx, cerr)
			(cx, cerr) = (dx, derr)

		# check if limit reached
		if (bx - ax) <= self.limit:
			#print(f'    fin: {cx} <{cerr}> || {ax}  {bx}')
			return (cx, cerr)
		else:
			#print(f'    cur: {cx} <{cerr}>')
			return self.findExtremum(ax, bx, cx, aerr, berr, cerr)

	def findExtrema(self, roots):
		control = vzeros(self.order + 2)
		control[0] = -1
		control[self.order + 1] = 1

		maxErr = 0.0

		for n in range(self.order):
			ax = roots[n]
			bx = roots[n+1]
			cx = ax + (bx - ax) * random.uniform(0.4, 0.6)
			aerr = self.evalError(ax)
			berr = self.evalError(bx)
			cerr = self.evalError(cx)
			#print(f'  find[{n}]: {ax}, {bx}')
			(rx, rerr) = self.findExtremum(ax, bx, cx, aerr, berr, cerr)
			assert(rx >= ax and rx <= bx) # extremum must be inside search range
			#print(f'  extrema[{n}]: {rx} <{rerr}>  ||  {ax} {bx}')

			control[n + 1] = rx
			maxErr = max(maxErr, rerr)

		return (control, maxErr)

	def scanRoots(self, numSteps):
		found = []
		coords = mp.linspace(-1.0, 1.0, numSteps)
		for ndx in range(len(coords)-1):
			ax = coords[ndx]
			bx = coords[ndx+1]
			aerr = self.evalEstimate(ax) - self.evalFunc(ax)
			berr = self.evalEstimate(bx) - self.evalFunc(bx)
			#print(f'bucket: {ax} <{aerr}>')
			#print(f'    to: {bx} <{berr}>')
			if mp.sign(aerr) != mp.sign(berr):
				(rx, rerr) = self.findRoot(ax, bx, aerr, berr)
				#print(f'  root in range: [{ax}, {bx}]: {rx} <{rerr}>')
				found.append((ax, bx, rx, rerr))
		return found

	def evalFunc(self, x):
		return self.func(x * self.k2 + self.k1)

	def evalWeight(self, x):
		return self.weightFunc(x * self.k2 + self.k1)

	def evalEstimate(self, x):
		sum = mp.mpf(0.0)
		for i in range(len(self.weights)):
			sum += self.weights[i] * mp.chebyt(i, x)
		return sum

	def evalError(self, x):
		return mp.fabs(self.evalEstimate(x) - self.evalFunc(x)) / self.evalWeight(x)

# Main

MAX_REMEZ_ITER = 10

# convert polynomials to assume input in [0.0, 1.0] range (instead of [-1.0, 1.0])
rebase = P.Polynomial([-1.0, 2.0])

def remezFit(name, func, weightFunc, domain, order):
	remez = Remez(func, weightFunc, domain, order)

	for iter in range(MAX_REMEZ_ITER):
		#print(f'ITER {iter}:')
		if remez.step():
			break

	return remez

def remezToPoly(remez):
	cheby = C.Chebyshev(remez.weights)
	p = cheby.convert(kind=P.Polynomial)
	p = p(rebase)
	# (x0, x1) = remez.domain
	# p = p(P.Polynomial([-x0 / (x1 - x0), 1.0 / (x1 - x0)]))
	return p

def writeCoefficients(file, name, maxError, order, segments):
	file.write('\n')

	if len(segments) == 1:
		precision = -math.log(maxError, 2.0)
		file.write(f'\t\t// Precision: {precision:.2f} bits\n')
		file.write('\t\t[MethodImpl(AggressiveInlining)]\n')
		file.write(f'\t\tpublic static int {name}Poly{order}(int a)\n')
		file.write('\t\t{\n')

		# get polynomial
		p = remezToPoly(segments[0])

		for ndx in reversed(range(order + 1)):
			c = p.coef[ndx]
			ic = (int)(c * (1 << 30) + 0.5)
			# file.write(f'\t\t\tconst int C{ndx} = {ic}; // {c}\n')

			if ndx == len(p.coef)-1:
				file.write(f'\t\t\tint y = Qmul30(a, {ic}); // {c}\n')
			elif ndx > 0:
				file.write(f'\t\t\ty = Qmul30(a, y + {ic}); // {c}\n')
			else:
				file.write(f'\t\t\ty = y + {ic}; // {c}\n')

		file.write('\t\t\treturn y;\n')

	else:
		numSegments = len(segments)
		precision = -math.log(maxError, 2.0)
		funcName = f'{name}Poly{order}Lut{numSegments}'
		tableName = f'{funcName}Table'

		# write constant table
		file.write(f'\t\tprivate static readonly int[] {tableName} =\n')
		file.write('\t\t{\n')

		for remez in segments:
			# get polynomial
			p = remezToPoly(remez)
			# map [x0, x1] to [0.0, 1.0]
			(x0, x1) = remez.domain
			p = p(P.Polynomial([-x0 / (x1 - x0), 1.0 / (x1 - x0)]))
			# write coefficients
			coefs = ' '.join(f'{int(c * (1<<30) + 0.5)},' for c in reversed(p.coef))
			file.write(f'\t\t\t{coefs}\n')

		file.write('\t\t};\n')

		# write function
		file.write('\n')
		file.write(f'\t\t// Precision: {precision:.2f} bits\n')
		file.write('\t\t[MethodImpl(AggressiveInlining)]\n')
		file.write(f'\t\tpublic static int {funcName}(int a)\n')
		file.write('\t\t{\n')

		segmentBits = int(math.log2(numSegments))
		file.write(f'\t\t\tint offset = (a >> {30 - segmentBits}) * {order + 1};\n')

		for ndx in reversed(range(order + 1)):
			if ndx == order:
				file.write(f'\t\t\tint y = Qmul30(a, {tableName}[offset + {0}]);\n')
			elif ndx > 0:
				file.write(f'\t\t\ty = Qmul30(a, y + {tableName}[offset + {order - ndx}]);\n')
			else:
				file.write(f'\t\t\ty = y + {tableName}[offset + {order}];\n')

		file.write('\t\t\treturn y;\n')

	file.write('\t\t}\n')
	file.flush()

def remezFitSegmented(name, func, weightFunc, domain, numSegments, order):
	(xMin, xMax) = domain
	maxError = 0.0

	segments = []
	for segment in range(numSegments):
		x0 = xMin + (xMax - xMin) * mp.mpf(segment) / numSegments
		x1 = xMin + (xMax - xMin) * mp.mpf(segment + 1) / numSegments
		remez = remezFit(name, func, weightFunc, (x0, x1), order)
		#print(f'    segment[{segment}]: {-math.log(remez.maxError, 2.0):.2f} bits [{x0} .. {x1}]')
		maxError = max(maxError, remez.maxError)
		segments.append(remez)

	return (maxError, segments)

epsilon = mp.mpf(f'1e-{dps//3}')

# Implementation of sin() with the assumption that input has been normalized to [-1.0, 1.0] range
# and squared. Also assumes the output will be multiplied by x once more (to make it odd).
# See: https://github.com/samhocevar/lolremez/wiki/Tutorial-3-of-5:-changing-variables-for-simpler-polynomials
def sinSqr(x):
	x = x * 0.25 * mp.pi * mp.pi
	xx = mp.sqrt(x)
	y = mp.sin(xx) / xx
	return y * 0.5*mp.pi

FUNCS = [
	('Exp', lambda x: mp.exp(x), lambda x: mp.exp(x), (0.0, 1.0)),
	('Exp2', lambda x: 2.0**x, lambda x: 2.0**x, (0.0, 1.0)),
	('Log', lambda x: mp.log(x+1), lambda x: mp.log(x+1) * (mp.log(2.0) - mp.log(x+1)), (epsilon, 1.0-epsilon)),
	('Log2', lambda x: mp.log(x+1, 2.0), lambda x: mp.log(x+1, 2.0) * (1 - mp.log(x+1, 2.0)), (epsilon, 1.0-epsilon)),
	('Rcp', lambda x: 1.0 / (x+1), lambda x: 1.0 / (x+1), (0.0, 1.0)),
	('Sqrt', lambda x: mp.sqrt(x+1), lambda x: mp.sqrt(x+1), (0.0, 1.0)),
	('RSqrt', lambda x: 1.0 / mp.sqrt(x+1), lambda x: 1.0 / mp.sqrt(x+1), (0.0, 1.0)),
	('Atan', lambda x: mp.atan(x), lambda x: mp.atan(x), (0.0, 1.0)),
	('Sin', sinSqr, sinSqr, (epsilon, 1.0)),
]

def generateCode():
	with open(f'fitted.txt', 'w', newline='\n') as file:
		file.write('\t// AUTO-GENERATED POLYNOMIAL APPROXIMATIONS\n')
		file.write('\n')
		file.write('\tpublic static class Util\n')
		file.write('\t{\n')

		for (name, func, weightFunc, domain) in FUNCS:
			print()
			print(f'{name}():')
			orderRange = range(3, 10) if name != 'Sin' else range(1, 5)
			# orderRange = range(3, 6) if name != 'sin' else range(1, 4)
			for order in orderRange:
				remez = remezFit(name, func, weightFunc, domain, order)
				print(f'  {name}<{order}>(): {-math.log(remez.maxError, 2.0):.2f} bits')
				writeCoefficients(file, name, remez.maxError, order, [remez])

			for numSegments in [4, 8, 16, 32]:
				orders = [2, 3, 4, 5] if name != 'Sin' else [1, 2, 3]
				# orders = [3, 4] if name != 'sin' else [1, 2, 3]
				for order in orders:
					(maxError, segments) = remezFitSegmented(name, func, weightFunc, domain, numSegments, order)
					print(f'  {name}<{order}>[{numSegments}](): {-math.log(maxError, 2.0):.2f} bits')
					writeCoefficients(file, name, maxError, order, segments)

		file.write('\t}\n')

def plotError():
	print('Plotting error..')
	#func = lambda x: 1.0 / (x + 1.0)
	func = sinSqr
	remez = remezFit('Sin', func, func, (epsilon, 1.0), 4)
	# remez = remezFit('Rcp', func, func, (1.0, 2.0), 3)
	print('err:', remez.maxError)
	est = remezToPoly(remez)
	err = lambda x: (func(x) - est(x)) / func(x)
	mp.plot([err], [0.0, 1.0])
	#mp.plot([func], [0.0, 1.0])

# Main

generateCode()
#plotError()
