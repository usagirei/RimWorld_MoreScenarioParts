import sys
import os
import jinja2
import argparse
from lxml import etree
		
templates = {}
env = jinja2.Environment(loader=jinja2.FileSystemLoader(os.path.join(os.path.dirname(os.path.abspath(__file__)),"Templates")))
env.lstrip_blocks = True
env.trim_blocks = True

def template_function(func):
    env.globals[func.__name__] = func
    return func

def template_filter(func):
    env.filters[func.__name__] = func
    return func

def getTemplate(name): 
	if name not in templates:
		template = env.get_template(name + ".j2")
		templates[name] = template
	return templates[name]

def getFiles(dir, *exts):
	for base, _, files in os.walk(dir):
		for f in [x for x in files if os.path.splitext(x)[1] in exts]:
			yield os.path.join(base, f)

class Dict(argparse.Action):
	def __call__(self, parser, namespace, values, opts, **kwargs):
		lst = getattr(namespace, self.dest)
		if lst is None:
			lst = {}
			setattr(namespace, self.dest, lst)
		key, value = values
		lst[key] = value

@template_filter
def sanitize_identifier(value):
	keywords = [
		"abstract",	"as", "base", "bool","break","byte","case","catch",
		"char","checked","class","const","continue","decimal","default","delegate",
		"do","double","else","enum","event","explicit","extern","false",
		"finally","fixed","float","for","foreach","goto","if","implicit",
		"in","int","interface","internal","is","lock","long","namespace",
		"new","null","object","operator","out","override","params","private",
		"protected","public","readonly","ref","return","sbyte","sealed","short",
		"sizeof","stackalloc","static","string","struct","switch","this","throw",
		"true","try","typeof","uint","ulong","unchecked","unsafe","ushort",
		"using","virtual","void","volatile","while"
	]
	if value in keywords:
		return f"@{value}"
	return value

def main(argv):
	
	parser = argparse.ArgumentParser(description='Processes XML Defs')

	parser.add_argument('indir', type=str, help='.xml Source Directory')
	parser.add_argument('outfile', type=str, help='Destination File')
	parser.add_argument('template', type=str, help='Template File Name')
	parser.add_argument('xpath', type=str, help='XPath Selector and Template')
	parser.add_argument('-d', '--define', type=str, nargs=2, action=Dict, metavar=('NAME', 'VALUE'), help='Extra Template Definitions')

	opts = parser.parse_args()
	os.makedirs(os.path.dirname(opts.outfile), exist_ok=True)
	if not opts.define:
		setattr(opts, 'define', [])
	
	files = list(getFiles(opts.indir, '.xml'))
	
	args = []
	for filename in files:
		with open(filename, encoding='utf8') as xf:
			doc = etree.parse(xf)
		elems = doc.xpath(opts.xpath)
		if elems:
			args.append((filename, elems))

	template = getTemplate(opts.template) # type: jinja2.Template
	rendered = template.render(args=args, **opts.define)

	with open(opts.outfile, "w+") as of:
		of.write(rendered)
	print(f"{opts.template} -> {opts.outfile}")

	pass

if __name__ == "__main__":
	main(sys.argv)