from setuptools import setup

setup(name='griot',
      version='0.1',
      description='Extracts the word-lemma-POS frequency from the raw COCA spreadsheet and prepares it for use within Bailiwick',
      author='Jeffrey M Farley',
      author_email='JeffreyMFarley@users.noreply.github.com',
      packages=['griot'],
      test_suite='tests',
      zip_safe=False)
